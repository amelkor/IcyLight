using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml;
using CommandLine;
using IcyLightPacker.Tools;
using IcyLightPacker.Verbs;
using Serilog;

namespace IcyLightPacker
{
internal static class Program
    {
        private static readonly ILogger _log = Logger.Log;
        
        private static int Main(string[] args)
        {
            Parser.Default.ParseArguments<KeyOptions, PackOptions>(args)
                .WithParsed<KeyOptions>(RunKeysCreation)
                .WithParsed<PackOptions>(RunPackaging);

            return Codes.SUCCESS;
        }

        /// <summary>
        /// Create a key pair for packaging.
        /// </summary>
        /// <param name="options"></param>
        private static void RunKeysCreation(KeyOptions options)
        {
            try
            {
                _log.Information("Keys creation started");

                if (string.IsNullOrEmpty(options.OutPath))
                    options.OutPath = "./";
                
                if (!Directory.Exists(options.OutPath))
                    Directory.CreateDirectory(options.OutPath);

                var (privateKey, publicKey) = Ed25519KeyGenerator.Create();

                var privPath = Path.Combine(options.OutPath, "private_key.json");
                var pubPath = Path.Combine(options.OutPath, "public_key.json");

                File.WriteAllText(privPath, JsonSerializer.Serialize(privateKey), Encoding.UTF8);
                File.WriteAllText(pubPath, JsonSerializer.Serialize(publicKey), Encoding.UTF8);

                _log.Information($"Keys have been created: {Path.GetFullPath(privPath)} {Path.GetFullPath(pubPath)}");
            }
            catch (IOException e)
            {
                _log.Error($"unable to create keys: {e.Message}");
                Environment.Exit(Codes.ERROR_IO_EXCEPTION);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Error($"unable to create keys: {e.Message}");
                Environment.Exit(Codes.ERROR_ACCESS_EXCEPTION);
            }
        }

        /// <summary>
        /// Produce an update package using a ED25519 private key to create signatures.
        /// </summary>
        /// <param name="options"></param>
        private static void RunPackaging(PackOptions options)
        {
            #region validate args

            if (!Directory.Exists(options.SourcePath))
            {
                _log.Fatal("Source directory doesn't exist");
                Environment.Exit(Codes.ERROR_SRC_DIR_MISSING);
            }

            if (Directory.Exists(options.TargetPath) && Directory.GetFiles(options.TargetPath).Any())
            {
                _log.Error("Out directory is not empty");
                Environment.Exit(Codes.ERROR_OUT_DIR_NOT_EMPTY);
            }

            if (string.IsNullOrEmpty(options.Ed25519PrivateKeyPath))
            {
                options.Ed25519PrivateKeyPath = "private_key.json";
            }
            
            if (!File.Exists(options.Ed25519PrivateKeyPath))
            {
                _log.Fatal("Provided Ed25519 Private Key file doesn't exist");
                Environment.Exit(Codes.ERROR_PRIV_KEY_MISSING);
            }

            if (string.IsNullOrEmpty(options.PackageInfoSavePath))
            {
                options.PackageInfoSavePath = $"{options.TargetPath}";
                _log.Warning($"Output path for packageInfo was not provided. Using default path {options.PackageInfoSavePath}");
            }

            #endregion

            _log.Information($"Package creation started: AppId: {options.ApplicationId} Version: {options.Version}");
            _log.Information($"Source path: {options.SourcePath}");
            _log.Information($"Output path: {options.TargetPath}");

            var package = new Package(options);
            var packageInfo = package.Build();

            packageInfo.SaveToFile(options.PackageInfoSavePath);
        }
    }
}