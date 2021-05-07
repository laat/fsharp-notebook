#!/usr/bin/env -S dotnet fsi --quiet

open System.Xml.XPath

// Wach out for XML namespaces. They need special consideration

let doc = XPathDocument("./input/document.xml")
let nav = doc.CreateNavigator()


nav.Select("//bookstore//book/title")
|> Seq.unfold
    (fun it ->
        match it.MoveNext() with
        | false -> None
        | true -> Some(it.Current, it))
|> Seq.take 1
|> Seq.iter (printfn "%A")

let author =
    nav.SelectSingleNode("//bookstore//book[1]/author/first-name")

printfn "%s" author.Value

type XPathNodeIterator with
    /// Converts an XPathNodeIterator into a seq{XPathNavigator}.
    member this.AsEnumerable =
        this
        |> Seq.unfold
            (fun it ->
                match it.MoveNext() with
                | false -> None
                | true -> Some(it.Current, it))

module XPathNodeIterator =
    let toSeq (x: System.Xml.XPath.XPathNodeIterator) =
        x
        |> Seq.unfold
            (fun it ->
                match it.MoveNext() with
                | false -> None
                | true -> Some(it.Current, it))

nav.Select("//bookstore//book/title")
|> XPathNodeIterator.toSeq
|> Seq.take 1
|> Seq.iter (printfn "%A")
