#!/usr/bin/env -S dotnet fsi --quiet

open System.Xml.XPath

module XPathNodeIterator =
  let toSeq =
    Seq.unfold
      (fun (it: System.Xml.XPath.XPathNodeIterator) ->
        match it.MoveNext() with
        | false -> None
        | true -> Some(it.Current, it))

// Wach out for XML namespaces. They need special consideration
let doc = XPathDocument("./input/document.xml")
let nav = doc.CreateNavigator()

let author =
  nav
    .SelectSingleNode(
      "//bookstore//book[1]/author/first-name"
    )
    .Value

printfn $"%s{author}"


nav.Select("//bookstore//book/title")
|> XPathNodeIterator.toSeq
|> Seq.take 1
|> Seq.iter (printfn "%A")
