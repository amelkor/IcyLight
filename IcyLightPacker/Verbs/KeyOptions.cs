using CommandLine;

// ReSharper disable ClassNeverInstantiated.Global

namespace IcyLightPacker.Verbs
{
    [Verb("--keys", HelpText = "Create new ED25519 keys")]
    public class KeyOptions
    {
        [Option('o', "out-path")]
        public string OutPath { get; set; }
    }
}