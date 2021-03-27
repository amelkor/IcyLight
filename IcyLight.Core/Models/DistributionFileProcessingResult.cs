namespace IcyLight.Core
{
    public readonly struct DistributionFileProcessingResult
    {
        public readonly FileProcessingResultCode Code;
        public readonly DistributionFile DistributionFile;

        public DistributionFileProcessingResult(FileProcessingResultCode code, DistributionFile distributionFile)
        {
            Code = code;
            DistributionFile = distributionFile;
        }
    }
}