#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: JsonSchema.Net"
// 
// This file contains an example of validation serialization and an exception
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

let document = // should not validate
  JsonDocument.Parse
    """
{
  "firstName": 123
}
"""

let validationResult =
  schema.Validate(
    document.RootElement,
    ValidationOptions(OutputFormat = OutputFormat.Basic, RequireFormatValidation = true)
  )

// When serialize to a JSON that's readable and well defined
// https://json-schema.org/draft/2019-09/json-schema-core.html#rfc.section.10
let validationJson = JsonSerializer.Serialize(validationResult, JsonSerializerOptions(WriteIndented = true))
(* 
Example:
{
  "valid": false,
  "keywordLocation": "#/properties/firstName/type",
  "instanceLocation": "#/firstName",
  "error": "Value is number but should be string"
}
*)

// Good for exception messages?
if not validationResult.IsValid then
  failwithf "invalid JSON: %s" validationJson