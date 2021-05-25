#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: Azure.Storage.Blobs, 12.8.4"
#load "helpers.fsx"

// namespace Azure.Storage.Blobs

[<AutoOpen>]
module BlobClient =
    open System
    open System.IO
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
                    |> Async.AwaitTask
            }

        member this.AsyncExists() =
            async {
                let! token = Async.CancellationToken

                return!
                    this.ExistsAsync(cancellationToken = token)
                    |> Async.AwaitTask
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
                    |> Async.AwaitTask
            }

        member this.AsyncUpload
            (
                content: Stream,
                ?httpHeaders: BlobHttpHeaders,
                ?metadata: Collections.Generic.IDictionary<string, string>,
                ?conditions: BlobRequestConditions,
                ?progressHandler: IProgress<int64>,
                ?accessTier: Nullable<AccessTier>,
                ?transferOptions: Storage.StorageTransferOptions
            ) =
            async {
                let! token = Async.CancellationToken

                return!
                    this.UploadAsync(
                        content,
                        httpHeaders = defaultArg httpHeaders null,
                        metadata = defaultArg metadata null,
                        conditions = defaultArg conditions null,
                        progressHandler = defaultArg progressHandler null,
                        accessTier = defaultArg accessTier (Nullable()),
                        transferOptions = defaultArg transferOptions Unchecked.defaultof<_>,
                        cancellationToken = token
                    )
                    |> Async.AwaitTask
            }

        member this.AsyncUpload
            (
                content: string,
                ?httpHeaders: BlobHttpHeaders,
                ?metadata: Collections.Generic.IDictionary<string, string>,
                ?conditions: BlobRequestConditions,
                ?progressHandler: IProgress<int64>,
                ?accessTier: Nullable<AccessTier>,
                ?transferOptions: Storage.StorageTransferOptions
            ) =
            async {
                let! token = Async.CancellationToken

                return!
                    this.UploadAsync(
                        content,
                        httpHeaders = defaultArg httpHeaders null,
                        metadata = defaultArg metadata null,
                        conditions = defaultArg conditions null,
                        progressHandler = defaultArg progressHandler null,
                        accessTier = defaultArg accessTier (Nullable()),
                        transferOptions = defaultArg transferOptions Unchecked.defaultof<_>,
                        cancellationToken = token
                    )
                    |> Async.AwaitTask
            }

        member this.AsyncDeleteIfExists(?snapshotsOption: DeleteSnapshotsOption, ?conditions: BlobRequestConditions) =
            async {
                let! token = Async.CancellationToken

                return!
                    this.DeleteIfExistsAsync(
                        snapshotsOption = defaultArg snapshotsOption Unchecked.defaultof<_>,
                        conditions = defaultArg conditions null,
                        cancellationToken = token
                    )
                    |> Async.AwaitTask
            }
