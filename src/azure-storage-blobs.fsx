#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: Azure.Storage.Blobs, 12.8.4"
#load "helpers.fsx"

open Helpers
open Azure
open Azure.Storage.Blobs
open Azure.Storage.Blobs.Models


type BlobClient with
    member this.AsyncDownload(?range: HttpRange, ?conditions: BlobRequestConditions, ?rangeGetContentHash: bool) =
        async {
            let! token = Async.CancellationToken

            return!
                this.DownloadAsync(
                    range = defaultArg range Unchecked.defaultof<_>,
                    conditions = defaultArg conditions null,
                    rangeGetContentHash = defaultArg rangeGetContentHash false,
                    cancellationToken = token
                )
                |> Async.AwaitTaskCorrect
        }

    member this.AsyncExists() =
        async {
            let! token = Async.CancellationToken

            return!
                this.ExistsAsync(cancellationToken = token)
                |> Async.AwaitTaskCorrect
        }

    member this.AsyncDelete(?snapshotsOption: DeleteSnapshotsOption, ?conditions: BlobRequestConditions) =
        async {
            let! token = Async.CancellationToken

            return!
                this.DeleteAsync(
                    snapshotsOption = defaultArg snapshotsOption Unchecked.defaultof<_>,
                    conditions = defaultArg conditions null,
                    cancellationToken = token
                )
                |> Async.AwaitTaskCorrect
        }
