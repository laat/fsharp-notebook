#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: JsonSchema.Net"
//
// This file contains an example of validating against a subschema defined in #/$defs/veggies
//

open Json.Schema
open System.Text.Json

module JsonSchema =
  /// get schema from <c>#/$defs/typeName</c> or <c>#/definitions/typeName</c>
  let tryGetDefinition typeName (schema: JsonSchema) =
    schema.Keywords
    |> Seq.choose
         (function
         | :? DefsKeyword as res -> Some res.Definitions // JSON Schema Draft 2020-12 $defs
         | :? DefinitionsKeyword as res -> Some res.Definitions // Old definitions keyword
         | _ -> None)
    |> Seq.choose
         (fun x ->
           match x.TryGetValue(typeName) with
           | true, x -> Some x
           | _ -> None)
    |> Seq.tryHead

let schema =
  JsonSchema.FromText
    """
{
  "$id": "https://example.com/arrays.schema.json",
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "$defs": {
    "fruit": { "type": "string" },
    "veggie": {
      "type": "object",
      "required": [ "veggieName", "veggieLike" ],
      "properties": {
        "veggieName": {
          "type": "string",
          "description": "The name of the vegetable."
        },
        "veggieLike": {
          "type": "boolean",
          "description": "Do I like this vegetable?"
        }
      }
    }
  }
}
"""


let document =
  JsonDocument.Parse
    """
{
  "veggieName": "potato",
  "veggieLike": true
}
"""

let veggieSchema =
  schema
  |> JsonSchema.tryGetDefinition "veggie"
  |> Option.get

printfn "document is valid: %A" (veggieSchema.Validate(document.RootElement).IsValid)
