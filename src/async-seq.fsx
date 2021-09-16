#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: FSharp.Control.AsyncSeq, 3.1.0"
#r "nuget: FSharp.Control.Reactive, 5.0.2"

open System
open FSharp.Control
open FSharp.Control.Reactive

let subject = Subject.behavior "1"
let stream = AsyncSeq.ofObservableBuffered subject

let task =
    stream
    |> AsyncSeq.take 2
    |> AsyncSeq.iter (fun x -> printfn "%s" x)
    |> Async.StartImmediateAsTask

printfn "Started task"

subject.OnNext "2"

task |> Async.AwaitTask |> Async.RunSynchronously
