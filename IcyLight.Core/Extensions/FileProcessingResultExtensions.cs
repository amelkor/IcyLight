namespace IcyLight.Core
{
    public static class FileProcessingResultExtensions
    {
        public static bool IsSuccessfull(this FileProcessingResultCode fileProcessingResult) =>
            fileProcessingResult == FileProcessingResultCode.Prepared ||
            fileProcessingResult == FileProcessingResultCode.Installed ||
            fileProcessingResult == FileProcessingResultCode.Updated ||
            fileProcessingResult == FileProcessingResultCode.SameVersionSkipped;
    }
}