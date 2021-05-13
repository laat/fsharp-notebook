#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: Ply"
// https://dev.to/tunaxor/making-http-requests-in-f-1n0b

open FSharp.Control.Tasks
open System.Net.Http
open System.IO

Directory.CreateDirectory("output")
let output = "./output/http-http-client.json"
let endpoint = "https://httpbin.org/get"

task {
  use file = File.OpenWrite(output)
  use client = new HttpClient()
  let! response = client.GetStreamAsync(endpoint)
  do! response.CopyToAsync file
}
|> Async.AwaitTask
|> Async.RunSynchronously
