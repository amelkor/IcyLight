using System;
using System.IO;
using System.Text.Json;

namespace IcyLight.Distribution.Impl
{
    [Serializable]
    public class Ed25519Key
    {
        public string Algorithm { get; set; }
        public string KeyType { get; set; }
        public string Key { get; set; }

        public Ed25519Key()
        {
        }

        public Ed25519Key(string algorithm, string keyType, string key)
        {
            Algorithm = algorithm;
            KeyType = keyType;
            Key = key;
        }

        public ReadOnlySpan<byte> ToBytes() => new(Convert.FromBase64String(Key));

        public override string ToString() => $"{Algorithm} {KeyType} {Key}";
        
        public static Ed25519Key FromFile(string path)
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