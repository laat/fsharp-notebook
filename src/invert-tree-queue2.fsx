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

// let tree =
//   [ 1 .. 105000 ]
//   |> List.fold (fun (root: Node option) i -> Some { Value = i; Left = None; Right = root }) None
//   |> Option.get

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

type Queue<'a> = private | Queue of list<'a> * list<'a>

module Queue =
  let empty = Queue([], [])
  let isEmpty (Queue (front, back)) = List.isEmpty front && List.isEmpty back
  let enqueue x (Queue (front, back)) = Queue(front, x :: back)

  let dequeue (Queue (front, back)) =
    match front, back with
    | x :: front, back -> Some x, Queue(front, back)
    | [], back ->
        match List.rev back with
        | [] -> None, Queue([], [])
        | h :: t -> Some h, Queue(t, [])

let rec bfs result (queue: Queue<Node>) =
  match Queue.dequeue queue with
  | None, _ -> result
  | Some node, queue ->
      let queue =
        if node.Right.IsSome then
          Queue.enqueue (node.Right.Value) queue
        else
          queue

      let queue =
        if node.Left.IsSome then
          Queue.enqueue (node.Left.Value) queue
        else
          queue

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

Queue.empty
|> Queue.enqueue tree
|> bfs []
|> swap Map.empty
|> Map.find (Some tree)
|> printDfs "inverted"
