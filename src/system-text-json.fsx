#!/usr/bin/env -S dotnet fsi --quiet
#r "System.Runtime.Serialization"
#r "nuget: FSharp.SystemTextJson"

open System.IO
open System.Text.Json
open System.Text.Json.Serialization


[<RequireQualifiedAccess>]
module JsonSerializer =
    let inline serialize options obj = JsonSerializer.Serialize(obj, options)

    let inline deserialize<'t> options (jsonString: string) : 't =
        JsonSerializer.Deserialize<'t>(jsonString, options)

    let deserializeAsync<'t> options (stream: Stream) : Async<'t> =
        async {
            let! token = Async.CancellationToken

            return!
                JsonSerializer
                    .DeserializeAsync<'t>(stream, options, cancellationToken = token)
                    .AsTask()
                |> Async.AwaitTask
        }

let options = JsonSerializerOptions()
options.Converters.Add(JsonFSharpConverter())

type X =
    { [<JsonPropertyName("foo")>]
      Bar: int option }

"""{"foo": 1, "lol": 1}"""
|> JsonSerializer.deserialize<X> options
|> printfn "%A"

{ Bar = Some 100 }
|> JsonSerializer.serialize options
|> printfn "%s"
