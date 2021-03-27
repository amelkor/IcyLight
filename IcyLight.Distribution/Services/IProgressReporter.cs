using System;

namespace IcyLight.Distribution.Services
{
    public interface IProgressReporter
    {
        event Action<long, long> ProgressChanged;
    }
}