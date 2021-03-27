using System.Collections.Concurrent;
using System.Linq;

namespace IcyLight.Core
{
    public class DistributionResult
    {
        private readonly DistributionPackage _package;

        public readonly ConcurrentBag<DistributionFileProcessingResult> succeded = new();
        public readonly ConcurrentBag<DistributionFileProcessingResult> failed  = new();

        public DistributionResultCode Code { get; private set; }
        public bool Successfull => failed.IsEmpty && _package.DistributionFiles.Count() == succeded.Count;
        
        public DistributionResult(DistributionPackage package)
        {
            _package = package;
        }

        public DistributionResult SetCode(DistributionResultCode code)
        {
            Code = code;
            return this;
        }
    }
}