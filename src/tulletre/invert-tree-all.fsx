#!/usr/bin/env -S dotnet fsi --quiet
type Node =
    { Value: int
      Left: Node option
      Right: Node option }

let root =
    { Value = 9
      Left =
          Some
              { Value = 3
                Left = Some { Value = 1; Right = None; Left = None }
                Right = Some { Value = 4; Right = None; Left = None } }
      Right = Some { Value = 5; Left = None; Right = None } }

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

root |> printDfs "original" // [9; 3; 1; 4; 5]

let invertTree (root: Node) =
    let rec invert =
        function
        | None -> None
        | Some n ->
            Some
                { Value = n.Value
                  Left = invert n.Right
                  Right = invert n.Left }

    invert (Some root) |> Option.get

root |> invertTree |> printDfs "inverted simple" // [9; 5; 3; 4; 1]

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

let invertTree2 root =
    root
    |> Queue.enqueue Queue.empty
    |> bfs []
    |> swap Map.empty
    |> Map.find (Some root)

root
|> invertTree2
|> printDfs "inverted BFS-SWAP"

let invertTree3 root =
    let rec invert node (continuation: Node option -> Node option) =
        match node with
        | None -> continuation None
        | Some n ->
            invert
                n.Left
                (fun left ->
                    invert
                        n.Right
                        (fun right ->
                            continuation (
                                Some(
                                    { Value = n.Value
                                      Left = right
                                      Right = left }
                                )
                            )))

    invert (Some root) id |> Option.get


root |> invertTree3 |> printDfs "inverted CSP"

type ContinuationBuilder() =
    member this.Return(x) = (fun k -> k x)
    member this.Bind(m, f) = (fun k -> m (fun a -> f a k))

let cps = ContinuationBuilder()

let invertTree4 root =
    let rec invert node =
        cps {
            match node with
            | None -> return None
            | Some n ->
                let! left = invert n.Left
                let! right = invert n.Right

                return
                    Some
                        { Value = n.Value
                          Right = left
                          Left = right }
        }

    invert (Some root) id |> Option.get

root |> invertTree4 |> printDfs "inverted CSP CE"
