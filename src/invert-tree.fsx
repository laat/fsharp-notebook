#!/usr/bin/env -S dotnet fsi --quiet
type Node =
  { Value: int
    Left: Node option
    Right: Node option }

let tree =
  { Value = 9
    Left =
      Some
        { Value = 3
          Left = Some { Value = 1; Right = None; Left = None }
          Right = Some { Value = 4; Right = None; Left = None } }
    Right = Some { Value = 5; Left = None; Right = None } }

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

// // FIXME: implement invertion of the tree
// let invertTree (root: Node) = root

// // expected: [9; 5; 3; 4; 1]
// tree |> invertTree |> printDfs "inverted"

let rec invertTree node =
  match node with
  | Some n ->
      Some
        { Value = n.Value
          Left = invertTree n.Right
          Right = invertTree n.Left }
  | None -> None

Some tree
|> invertTree
|> Option.get
|> printDfs "inverted"
