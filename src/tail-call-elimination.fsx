#!/usr/bin/env -S dotnet fsi --quiet
// add to disable tail calls --tailcalls-

// let addLots1 =
//     id
//     |> fun next x -> next (x + 1L)
//     |> fun next x -> next (x + 2L)
//     |> fun next x -> next (x + 3L)
// ... to 10_000_000
let addLots1 =
    [ for i in 1 .. 10_000_000 do
          fun next x -> next (x + (int64 i)) ]
    |> List.fold (|>) id

// works: 50000005000010
addLots1 10L |> printfn "%d"

// let addLots2 =
//     id
//     >> fun x -> x + 1L
//     >> fun x -> x + 2L
//     >> fun x -> x + 3L
// ... to 10_000_000
let addLots2 =
    [ for i in 1 .. 10_000_000 do
          fun x -> x + (int64 i) ]
    |> List.fold (>>) id

// stack overflows
addLots2 10L |> printfn "%d"
