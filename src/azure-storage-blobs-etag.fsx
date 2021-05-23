#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: Microsoft.Extensions.Caching.Memory, 5.0.0"
#r "nuget: Azure.Storage.Blobs, 12.8.4"
#r "nuget: Ply"
#load "helpers.fsx"

open Helpers
open System
open System.Threading
open Microsoft.Extensions.Caching.Memory
open Azure
open Azure.Storage.Blobs
open Azure.Storage.Blobs.Models
open FSharp.Control.Tasks


type private BlobDocument<'a> = { ETag: ETag; Value: 'a }

type EtagDownloader<'a>
    (
        cache: IMemoryCache,
        toCacheValue: (Response<BlobDownloadInfo> -> 'a),
        ?memoryCacheEntryOptions: MemoryCacheEntryOptions
    ) =

    let memoryCacheEntryOptions =
        defaultArg
            memoryCacheEntryOptions
            (MemoryCacheEntryOptions(SlidingExpiration = Nullable(TimeSpan.FromHours(2.))))


    member this.DownloadAsync(client: BlobClient, ?token: CancellationToken) =
        let cancellationToken = defaultArg token CancellationToken.None
        let cacheKey = client.Uri

        let setCache (value: Response<BlobDownloadInfo>) =
            let transformed = toCacheValue value
            cache.Set(cacheKey, transformed, memoryCacheEntryOptions)

        match cache.TryGetValue(cacheKey) with
        | GotValue v ->
            let { ETag = etag; Value = value } = v :?> BlobDocument<'a>

            task {
                try
                    let! response =
                        client.DownloadAsync(
                            conditions = BlobRequestConditions(IfNoneMatch = etag),
                            cancellationToken = cancellationToken
                        )

                    return response |> setCache
                with
                | :? RequestFailedException as ex when ex.Status = 304 -> return value
                | exn -> return Async.reraise exn
            }
        | _ ->
            task {
                let! response = client.DownloadAsync(cancellationToken = cancellationToken)

                return response |> setCache
            }

    member this.AsyncDownload(client: BlobClient) =
        async {
            let! token = Async.CancellationToken

            return!
                this.DownloadAsync(client, token = token)
                |> Async.AwaitTaskCorrect
        }
