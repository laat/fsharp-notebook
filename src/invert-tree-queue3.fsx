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

type Queue<'a> = Queue of list<'a> * list<'a>

module Queue =
  let empty = Queue([], [])
  let enqueue (Queue (front, back)) x = Queue(front, x :: back)

  let rec dequeue =
    function
    | Queue ([], []) -> None, Queue([], [])
    | Queue (x :: front, back) -> Some x, Queue(front, back)
    | Queue ([], back) -> dequeue (Queue(List.rev back, []))

let rec bfs result queue =
  match Queue.dequeue queue with
  | None, _ -> result
  | Some node, queue ->
      bfs
        (node :: result)
        ([ node.Right; node.Left ]
         |> List.choose id
         |> List.fold Queue.enqueue queue)

let rec swap computed nodes =
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

tree
|> Queue.enqueue Queue.empty
|> bfs []
|> swap Map.empty
|> Map.find (Some tree)
|> printDfs "inverted"
