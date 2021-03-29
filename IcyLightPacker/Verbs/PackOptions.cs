using CommandLine;

// ReSharper disable ClassNeverInstantiated.Global

namespace IcyLightPacker.Verbs
{
    [Verb("--pack", HelpText = "Create an update package")]
    public class PackOptions
    {
        [Option('s', "source-path", Required = true, HelpText = "Set the path for input directory where built app files are located")]
        public string SourcePath { get; set; }

        [Option('t', "target-path", Required = false, HelpText = "Set the path for where to put the compressed app files")]
        public string TargetPath { get; set; }

        [Option('p', "package-info-path", HelpText = "Set the path where to put the resulting package info file")]
        public string PackageInfoSavePath { get; set; }

        [Option('k', "private-key-path", Required = false, HelpText = "Set the path where to get the ED25519 private key to sign package files")]
        public string Ed25519PrivateKeyPath { get; set; }

        [Option('f', "files-pattern", Required = false, HelpText = "The files pattern to refine which file types should be included into the package")]
        public string FilesPattern { get; set; } = "*";

        [Option('v', "version", Required = true, HelpText = "The application package version")]
        public string Version { get; set; }

        [Option('a', "app-id", Required = true, HelpText = "The application id")]
        public string ApplicationId { get; set; }

        [Option('o', "one-piece", Required = false, HelpText = "If true will compress the package into single zip file, false - each file compressed separately")]
        public bool CompressSingleZip { get; set; } = false;
    }
}