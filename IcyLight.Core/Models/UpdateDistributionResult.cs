using System;
using System.Collections.Generic;

namespace IcyLight.Core
{
    [Serializable]
    public class UpdateDistributionResult
    {
        public List<DistributionResult> UpdatePackages { get; set; } = new();
    }
}