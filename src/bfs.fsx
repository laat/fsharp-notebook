#!/usr/bin/env -S dotnet fsi --quiet
open System.Collections.Generic

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

let rec dfs (res: Node list) (stack: Node list) =
  match stack with
  | [] -> res
  | head :: tail ->
      dfs
        (head :: res)
        (([ head.Left; head.Right ] |> List.choose id)
         @ tail)

let rec bfs (queue: Queue<Node>) res =
  if queue.Count = 0 then
    res
  else
    let node = queue.Dequeue()

    if node.Left.IsSome then
      queue.Enqueue(node.Left.Value)

    if node.Right.IsSome then
      queue.Enqueue(node.Right.Value)

    bfs queue (node :: res)

let rec swap computed (nodes: Node list) =
  match nodes with
  | [] -> computed
  | head :: tail ->
      swap
        (Map.add
          (Some head)
          { Value = head.Value
            Left = Map.tryFind head.Right computed
            Right = Map.tryFind head.Left computed }
          computed)
        tail

let printDfs prefix node =
  [ node ]
  |> dfs []
  |> List.rev
  |> List.map (fun x -> x.Value)
  |> printfn "%s %A" prefix


tree |> printDfs "original"

bfs (Queue([ tree ])) []
|> swap Map.empty
|> Map.find (Some tree)
|> printDfs "inverted"
