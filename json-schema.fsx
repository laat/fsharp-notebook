#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: JsonSchema.Net"
//
// This file contains a basic example of json schema validation
//

open Json.Schema
open System.Text.Json

let schema =
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

let document =
  JsonDocument.Parse
    """
{
  "firstName": "John",
  "lastName": "Doe",
  "age": 21
}
"""

printfn "document is valid: %A" (schema.Validate(document.RootElement).IsValid)
