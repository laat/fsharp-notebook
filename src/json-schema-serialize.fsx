#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: JsonSchema.Net"
#r "nuget: FSharp.SystemTextJson"
//
// This file contains a basic example of json schema validation
//

[<RequireQualifiedAccess>]
module JsonSchema =
  open System.Text.Json
  open Json.Schema

  let private validateSerializeOptions =
    JsonSerializerOptions(WriteIndented = true)

  let private validateOptions =
    ValidationOptions(OutputFormat = OutputFormat.Basic, RequireFormatValidation = true)

  let checkJsonElement' options (schema: JsonSchema) root =
    let result = schema.Validate(root, options)

    if not (result.IsValid) then
      JsonSerializer.Serialize(result, validateSerializeOptions)
      |> failwithf "JSON Schema validation error %s"

  let checkJson' options (schema: JsonSchema) (text: string) =
    JsonDocument.Parse(text).RootElement
    |> checkJsonElement' options schema

    text

  let checkJson schema text = checkJson' validateOptions schema text

type Person =
  { FirstName: string
    LastName: string
    Age: int }

module Person =
  open System.Text.Json
  open System.Text.Json.Serialization
  open Json.Schema

  let private schema =
    JsonSchema.FromText
      """
  {
    "$id": "https://example.com/person.schema.json",
    "$schema": "https://json-schema.org/draft/2020-12/schema",
    "title": "Person",
    "type": "object",
    "properties": {
      "firstName": {
        "type": "string",
        "description": "The person's first name."
      },
      "lastName": {
        "type": "string",
        "description": "The person's last name."
      },
      "age": {
        "description": "Age in years which must be equal to or greater than zero.",
        "type": "integer",
        "minimum": 0
      }
    }
  }
  """

  let private serializerOptions =
    let options =
      JsonSerializerOptions(WriteIndented = true)

    options.Converters.Add(JsonFSharpConverter())
    options

  let serialize (p: Person) =
    // looking forward to
    // https://github.com/dotnet/runtime/pull/51025
    JsonSerializer.Serialize(
      {| firstName = p.FirstName
         lastName = p.LastName
         age = p.Age |},
      serializerOptions
    )
    |> JsonSchema.checkJson schema



{ FirstName = "John"
  LastName = "Doe"
  Age = 21 }
|> Person.serialize
|> printfn "%s"
