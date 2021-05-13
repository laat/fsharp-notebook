#!/usr/bin/env -S dotnet fsi --quiet
#r "System.Runtime.Serialization"
#r "nuget: FSharp.SystemTextJson"

open System.Text.Json
open System.Text.Json.Serialization

let options = JsonSerializerOptions()
options.Converters.Add(JsonFSharpConverter())

let json<'t> (myObj: 't) =
  JsonSerializer.Serialize(myObj, options)


let unjson<'t> (jsonString: string) : 't =
  JsonSerializer.Deserialize<'t>(jsonString, options)

type X =
  {
    [<JsonPropertyName("foo")>]
    Bar: int option
  }

let jsonString = """{"foo": 1, "lol": 1}"""

printfn "%A" (unjson<X> jsonString)
printfn "%A" (json { Bar = Some 100 })
