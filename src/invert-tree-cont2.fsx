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

type ContinuationBuilder() =
  member this.Return(x) = (fun k -> k x)
  member this.ReturnFrom(x) = x
  member this.Bind(m, f) = (fun k -> m (fun a -> f a k))
  member this.Delay(f) = (fun k -> f () k)

let cps = ContinuationBuilder()

let rec invertTree node =
  cps {
    match node with
    | Some n ->
        let! left = invertTree n.Left
        let! right = invertTree n.Right

        return
          Some
            { Value = n.Value
              Right = left
              Left = right }
    | None -> return None
  }


invertTree (Some tree) id
|> Option.get
|> printDfs "inverted"
