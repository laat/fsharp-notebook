#!/usr/bin/env -S dotnet fsi --quiet

module Domain =
    type AtLeastOne =
        private
            {
                A: int option
                B: int option
                C: int option
            }

    module AtLeastOne =

        let create aOpt bOpt cOpt =
            match aOpt, bOpt, cOpt with
            | (Some a, _, _) -> Some <| { A = aOpt; B = bOpt; C = cOpt }
            | (_, Some b, _) -> Some <| { A = aOpt; B = bOpt; C = cOpt }
            | (_, _, Some c) -> Some <| { A = aOpt; B = bOpt; C = cOpt }
            | _ -> None

        let createWhenAExists a bOpt cOpt = { A = Some a; B = bOpt; C = cOpt }
        let createWhenBExists aOpt b cOpt = { A = aOpt; B = Some b; C = cOpt }
        let createWhenCExists aOpt bOpt c = { A = aOpt; B = bOpt; C = Some c }

        let value atLeastOne =
            let a = atLeastOne.A
            let b = atLeastOne.B
            let c = atLeastOne.C
            {|A = a; B = b; C = c|}

open Domain

let theOne = AtLeastOne.createWhenAExists 1 None None
let theOneValue = theOne |> AtLeastOne.value
theOneValue.A // value on the anonymous record :D