module GiraffeJsonSchema

open System
open System.Text.Json
open Microsoft.AspNetCore.Http
open Giraffe
open Json.Schema

// assumes SystemTextJson.Serializer is used
let bindJsonSchema<'T> (schema: JsonSchema) (f: 'T -> HttpHandler) : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! body = ctx.ReadBodyFromRequestAsync()
            let voteJson = JsonDocument.Parse body

            let validationResult =
                schema.Validate(voteJson.RootElement, ValidationOptions(OutputFormat = OutputFormat.Basic))

            if not validationResult.IsValid then
                ctx.SetStatusCode 400
                return! json validationResult next ctx
            else
                let vote = voteJson.Deserialize<'T>()
                return! f vote next ctx
        }
