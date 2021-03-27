using System;
using System.Collections.Generic;

namespace IcyLight.Core
{
    [Serializable]
    public class UpdatePackage : DistributionPackage
    {
        /// <summary>
        /// Whether the update is mandatory or could be skipped.
        /// </summary>
        public bool Mandatory { get; set; }

        public int UpdateId { get; set; }
        public int PreviousUpdateId { get; set; }
    }
}