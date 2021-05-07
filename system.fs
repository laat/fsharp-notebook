
[<RequireQualifiedAccess>]
module Option =
    open System
    let inline ofNullable (x: 'a Nullable) = if x.HasValue then Some x.Value else None
    let inline toNullable (x: 'a option) = match x with | Some x -> Nullable<_> x | None -> Nullable<_> null
    let inline ofResult x = match x with Ok x -> Some x | Error _ -> None
    let inline toResult x err = match x with Some x -> Ok x | None -> Error err

[<RequireQualifiedAccess>]
module Nullable =
    open System
    let inline ofOption (x: 'a option) = match x with | Some x -> Nullable<_> x | None -> Nullable<_> null
    let inline toOption (x: 'a Nullable) = if x.HasValue then Some x.Value else None
    let inline ofResult x = match x with Ok x -> Nullable<_> x | Error _ -> Nullable<_> null
    let inline toResult (x: 'a Nullable) err = if x.HasValue then Ok x.Value else Error err

[<RequireQualifiedAccess>]
module Result =
    open System
    let inline ofOption x err = match x with Some x -> Ok x | None -> Error err
    let inline toOption x = match x with Ok x -> Some x | Error _ -> None
    let inline toNullable x = match x with Ok x -> Nullable<_> x | Error _ -> Nullable<_> null
    let inline ofNullable (x: 'a Nullable) err = if x.HasValue then Ok x.Value else Error err
    let traverse (f: 'a -> Result<'b, 'err>) (lst: 'a list): Result<'b list, 'err> =
        let initState: Result<'b list, 'err> = Ok []
        let folder (res: Result<'b list, 'err>) (next: 'a) =
            match res with
            | Ok lst -> Result.bind (fun t -> Ok (lst @ [t])) (f next)
            | Error err -> Error err
        List.fold folder initState lst

[<RequireQualifiedAccess>]
module String =
    open System
    let inline contains (y: string) (x: string) = x.Contains(y)
    let inline endsWith (y: string) (x: string) = x.EndsWith(y)
    let inline isEmpty (x: string) = x.Length < 1
    let inline isNullOrEmpty x = String.IsNullOrEmpty x
    let inline isNullOrWhiteSpace x = String.IsNullOrWhiteSpace x
    let inline join (s: string) (xs: string[]) = String.Join(s, xs)
    let inline replace (old': string) (new': string) (x: string) = x.Replace(old', new')
    let inline spilt (y: string) (x: string) = x.Split(y)
    let inline startsWith (y: string) (x: string) = x.StartsWith(y)
    let inline toCharArray (x: string) = x.ToCharArray()
    let inline toLowerInvariant (x: string) = x.ToLowerInvariant()
    let inline toUpperInvariant (x: string) = x.ToUpperInvariant()
    let inline trim (x: string) = x.Trim()
    let inline trimEnd (x: string) = x.TrimEnd()
    let inline trimStart (x: string) = x.TrimStart()
    let inline emptyToOption (x: string) = if isEmpty x then None else Some x 
    let inline nullOrEmptyToOption (x: string) = if isNullOrEmpty x then None else Some x 
    let inline nullOrWhiteSpaceToOption (x: string) = if isNullOrWhiteSpace x then None else Some x 
    let inline toEnumIgnoreCase<'a when 'a :> Enum and 'a: struct and 'a: (new : unit -> 'a)> (a: string) =
        let ok, v = Enum.TryParse<'a>(a, true)
        if ok then Some v else None

    let inline toEnum<'a when 'a :> Enum and 'a: struct and 'a: (new : unit -> 'a)> (a: string) =
        let ok, v = Enum.TryParse<'a>(a)
        if ok then Some v else None


    type String with
        /// Strongly-typed shortcut for Enum.TryParse(). Defaults to ignoring case.
        member this.ToEnum<'a when 'a :> Enum and 'a: struct and 'a: (new : unit -> 'a)>(?ignoreCase) =
            let ok, v =
                Enum.TryParse<'a>(this, defaultArg ignoreCase true)
            if ok then Some v else None


[<RequireQualifiedAccess>]
module Async =
    open System
    let inline bind f x = async.Bind(x, f)
    let inline map f x = bind (f >> async.Return) x
    let loopSleep (sleep: TimeSpan) fn = 
        let rec loop' () = 
            async {
                do! fn ()
                do! Async.Sleep sleep
                return! loop'()
            }
        loop' ()

[<RequireQualifiedAccess>]
module Seq =
    let inline mapAsync f xs = Seq.map (Async.map f) xs
    let inline iterAsync f xs =
        async {
            for a in xs do
                let! a = a
                f a
        }

[<RequireQualifiedAccess>]
module Uri =
    open System
    let inline absolutePath (x: Uri) = x.AbsolutePath
    let inline absoluteUri (x: Uri) = x.AbsoluteUri
    let inline authority (x: Uri) = x.Authority
    let inline dnsSafeHost (x: Uri) = x.DnsSafeHost
    let inline fragment (x: Uri) = x.Fragment
    let inline host (x: Uri) = x.Host
    let inline hostNameType (x: Uri) = x.HostNameType
    let inline idnHost (x: Uri) = x.IdnHost
    let inline isAbsoluteUri (x: Uri) = x.IsAbsoluteUri
    let inline isDefaultPort (x: Uri) = x.IsDefaultPort
    let inline isFile (x: Uri) = x.IsFile
    let inline isLoopback (x: Uri) = x.IsLoopback
    let inline isUnc (x: Uri) = x.IsUnc
    let inline localPath (x: Uri) = x.LocalPath
    let inline originalString (x: Uri) = x.OriginalString
    let inline pathAndQuery (x: Uri) = x.PathAndQuery
    let inline port (x: Uri) = x.Port
    let inline query (x: Uri) = x.Query
    let inline scheme (x: Uri) = x.Scheme
    let inline segments (x: Uri) = x.Segments
    let inline userEscaped (x: Uri) = x.UserEscaped
    let inline userInfo (x: Uri) = x.UserInfo

module Exception =
    open System
    open System.Runtime.ExceptionServices
    let reraise (x: Exception) =
        (ExceptionDispatchInfo.Capture x).Throw ()
        Unchecked.defaultof<_>

    type Exception with
        member this.Reraise () =
            (ExceptionDispatchInfo.Capture this).Throw ()
            Unchecked.defaultof<_>

[<RequireQualifiedAccess>]
module Regex =
    open System.Text.RegularExpressions
    let replace pattern replacement input =
        Regex.Replace(input, pattern, (replacement: string))

[<RequireQualifiedAccess>]
module DateTime = 
    open System
    let inline format (format: string) (d: DateTime) = d.ToString(format)

[<RequireQualifiedAccess>]
module DateTimeOffset = 
    open System
    let inline format (format: string) (d: DateTimeOffset) = d.ToString(format)