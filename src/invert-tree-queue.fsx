#!/usr/bin/env -S dotnet fsi --quiet
open System.Collections.Immutable

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

let rec bfs result (queue: ImmutableQueue<Node>) =
  if queue.IsEmpty then
    result
  else
    let queue, node = queue.Dequeue()
    let queue = if node.Right.IsSome then queue.Enqueue(node.Right.Value) else queue
    let queue = if node.Left.IsSome then queue.Enqueue(node.Left.Value) else queue
    bfs (node :: result) queue

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

(ImmutableQueue.Create(tree))
|> bfs []
|> swap Map.empty
|> Map.find (Some tree)
|> printDfs "inverted"
