using System;
using System.IO;
using System.Text.Json;
using IcyLight.Distribution.Impl;

namespace IcyLightPacker.Extensions
{
    public static class Ed25519KeyExtensions
    {
        public static Ed25519Key FromFile(this Ed25519Key key, string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("Key file path is invalid", nameof(path));

            var json = File.ReadAllText(path);

            if (string.IsNullOrEmpty(json))
                throw new ArgumentException("Provided file contains no data", nameof(path));

            return JsonSerializer.Deserialize<Ed25519Key>(json);
        }
    }
}