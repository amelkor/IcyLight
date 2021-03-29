using Serilog;

namespace IcyLightPacker
{
    public class Logger
    {
        internal static readonly ILogger Log = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Verbose().CreateLogger();
    }
}