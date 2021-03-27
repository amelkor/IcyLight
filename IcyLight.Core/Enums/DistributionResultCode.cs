namespace IcyLight.Core
{
    public enum DistributionResultCode
    {
        Unknown = 0,
        Finished,
        NoUpdates,
        Skipped,
        
        HasFailedFiles,
        NotEnoughSpace,
        PackageInfoNotFound,
        GeneralFailure
    }
}