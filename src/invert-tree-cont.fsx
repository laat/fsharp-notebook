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
    [ 1 .. 105000 ]
    |> List.fold (fun (root: Node option) i -> Some { Value = i; Left = None; Right = root }) None
    |> Option.get

let printDfs prefix node =
    let rec dfs result (stack: Node list) =
        match stack with
        | [] -> result
        | head :: tail ->
            dfs
                (head :: result)
                (([ head.Left; head.Right ] |> List.choose id)
                 @ tail)

    [ node ]
    |> dfs []
    |> List.rev
    |> List.map (fun x -> x.Value)
    |> printfn "%s %A" prefix


let rec invertTree node (continuation: Node option -> Node option) =
    match node with
    | Some n ->
        invertTree
            n.Left
            (fun left ->
                invertTree
                    n.Right
                    (fun right ->
                        continuation (
                            Some(
                                { Value = n.Value
                                  Left = right
                                  Right = left }
                            )
                        )))
    | None -> continuation None


invertTree (Some tree) id
|> Option.get
|> printDfs "inverted"
