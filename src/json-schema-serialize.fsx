#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: JsonSchema.Net"
//
// This file contains a basic example of json schema validation during serialization
//

[<AutoOpen>]
module Json =
  [<RequireQualifiedAccess>]
  module JsonSerializer =
    open System.Text.Json
    let inline serialize options obj = JsonSerializer.Serialize(obj, options)

[<AutoOpen>]
module JsonSchema =
  open System.Text.Json
  open Json.Schema

  [<RequireQualifiedAccess>]
  module ValidationResults =
    let checkIsValid (result: ValidationResults) =
      if not (result.IsValid) then
        result
        |> JsonSerializer.serialize (JsonSerializerOptions(WriteIndented = true))
        |> failwithf "JSON Schema validation error %s"


  [<RequireQualifiedAccess>]
  module JsonSchema =

    let validate (schema: JsonSchema) options root = schema.Validate(root, options)

    let checkJson (schema: JsonSchema) options (text: string) =
      use document = JsonDocument.Parse(text)

      document.RootElement
      |> validate schema options
      |> ValidationResults.checkIsValid

      text

type Person =
  { FirstName: string
    LastName: string
    Age: int }

module Person =
  open System.Text.Json
  open Json.Schema

  let private schema =
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
    |> JsonSchema.FromText

  let private validationOptions =
    ValidationOptions(OutputFormat = OutputFormat.Basic, RequireFormatValidation = true)


  // looking forward to
  // https://github.com/dotnet/runtime/pull/51025
  let toJson (p: Person) =
    {| firstName = p.FirstName
       lastName = p.LastName
       age = p.Age |}
    |> JsonSerializer.serialize (JsonSerializerOptions(WriteIndented = true))
    |> JsonSchema.checkJson schema validationOptions

{ FirstName = "John"
  LastName = "Doe"
  Age = 21 }
|> Person.toJson
|> printfn "%s"
