#!/usr/bin/env -S dotnet fsi --quiet
open System

[<CustomEquality; NoComparison>]
type Node =
  { Value: int
    Left: Node option
    Right: Node option }

  override a.Equals(obj) =
    let rec customCompare (a': Node option) (b': Node option) cont =
      match a', b' with
      | None, None -> cont true
      | Some _, None -> cont false
      | None, Some _ -> cont false
      | Some a, Some b when not (a.Value = b.Value) -> cont false
      | Some a, Some b ->
          customCompare
            a.Left
            b.Left
            (fun left -> customCompare a.Right b.Right (fun right -> cont (left && right && a.Value = b.Value)))

    match obj with
    | :? Node as b -> customCompare (Some a) (Some b) id
    | _ -> false

  override x.GetHashCode() =
    let rec getHashCode node cont =
      match node with
      | Some n ->
          getHashCode
            n.Left
            (fun left -> getHashCode n.Right (fun right -> cont (HashCode.Combine(hash (n.Value), left, right))))
      | None -> None |> hash |> cont

    getHashCode (Some x) id


let rightFooted =
  [ 1 .. 305000 ]
  |> List.fold (fun prev i -> Some { Value = i; Left = None; Right = prev }) None
  |> Option.get

printfn "right works: %b" (rightFooted = rightFooted)

let leftFooted =
  [ 1 .. 305000 ]
  |> List.fold (fun prev i -> Some { Value = i; Left = prev; Right = None }) None
  |> Option.get

printfn "left works: %b" (leftFooted = leftFooted)
printfn "rightHash %A" (rightFooted.GetHashCode())
printfn "leftHash %A" (leftFooted.GetHashCode())
