#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: Argu"
// http://fsprojects.github.io/Argu/tutorial.html

open Argu

type CliArguments =
    | Working_Directory of path: string
    | Listener of host: string * port: int
    | Data of base64: byte []
    | Port of tcp_port: int
    | Log_Level of level: int
    | Detach

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Working_Directory _ -> "specify a working directory."
            | Listener _ -> "specify a listener (hostname : port)."
            | Data _ -> "binary data in base64 encoding."
            | Port _ -> "specify a primary port."
            | Log_Level _ -> "set the log level."
            | Detach _ -> "detach daemon from console."

let programName = fsi.CommandLineArgs |> Array.head
let parser = ArgumentParser.Create<CliArguments>(programName = programName)

try
    let inputs = fsi.CommandLineArgs |> Array.tail
    let results = parser.Parse(inputs = inputs, raiseOnUsage = true)
    let all = results.GetAllResults()
    printfn "%A" all
with e -> printfn "%s" e.Message
