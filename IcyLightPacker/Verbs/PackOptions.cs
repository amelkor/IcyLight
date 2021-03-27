using CommandLine;

// ReSharper disable ClassNeverInstantiated.Global

namespace IcyLightPacker.Verbs
{
    [Verb("pack", HelpText = "Create an update package")]
    public class PackOptions
    {
        [Option('s', "source-path", Required = true, HelpText = "Set the path for input directory where built app files are located")]
        public string SourcePath { get; set; }
        
        [Option('o', "out-path", Required = false, HelpText = "Set the path for where to put the compressed app files")]
        public string OutPath { get; set; }
        
        [Option('p', "package-info-path", HelpText = "Set the path where to put the resulting package info file")]
        public string PackageInfoSavePath { get; set; }
        
        [Option('k', "private-key-path", Required = false, HelpText = "Set the path where to get the ED25519 private key to sign package files")]
        public string Ed25519PrivateKeyPath { get; set; }

        [Option('f', "files-pattern", Required = false, HelpText = "The files pattern to refine which file types should be included into the package")]
        public string FilesPattern { get; set; } = "*";
    }
}