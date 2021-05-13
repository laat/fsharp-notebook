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

  let checkJson (schema: JsonSchema) (text: string) =
    let document = JsonDocument.Parse(text)

    let validation =
      schema.Validate(document.RootElement, validateOptions)

    if not (validation.IsValid) then
      failwithf "JSON Schema validation error %s" (JsonSerializer.Serialize(validation, validateSerializeOptions))

    text

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
