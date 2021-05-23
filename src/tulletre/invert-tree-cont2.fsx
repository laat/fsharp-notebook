#!/usr/bin/env -S dotnet fsi --quiet
type Node =
    { Value: int
      Left: Node option
      Right: Node option }

// let tree =
//   { Value = 9
//     Left =
//       Some
//         { Value = 3
//           Left = Some { Value = 1; Right = None; Left = None }
//           Right = Some { Value = 4; Right = None; Left = None } }
//     Right = Some { Value = 5; Left = None; Right = None } }

let tree =
    [ 1 .. 305000 ]
    |> List.fold (fun prev i -> Some { Value = i; Left = None; Right = prev }) None
    |> Option.get

let printDfs prefix node =
    let rec dfs (stack: Node list) result =
        match stack with
        | [] -> result
        | head :: tail ->
            dfs
                (([ head.Left; head.Right ] |> List.choose id)
                 @ tail)
                (head.Value :: result)

    dfs [ node ] []
    |> List.rev
    |> printfn "%s\n%A\n" prefix

type ContinuationBuilder() =
    member this.Return(x) = (fun k -> k x)
    member this.Bind(m, f) = (fun k -> m (fun a -> f a k))
    member this.Delay(mk) = fun c -> mk () c

let cps = ContinuationBuilder()

let rec invertTree node =
    cps {
        match node with
        | None -> return None
        | Some n ->
            let! left = invertTree n.Left
            let! right = invertTree n.Right

            return
                Some
                    { Value = n.Value
                      Right = left
                      Left = right }
    }

tree |> printDfs "original"

invertTree (Some tree) id
|> Option.get
|> printDfs "inverted"
