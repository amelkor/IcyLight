namespace IcyLight.Core
{
    public enum FileProcessingResultCode
    {
        NotProcessed = 0,
        Prepared,
        Installed,
        Updated,
        SameVersionSkipped,
        MarkedForDelete,
        
        DownloadFailed,
        DecompressionFailed,
        CompressedFileWrongSize,
        DecompressedFileWrongSize,
        SignatureFailed
    }
}