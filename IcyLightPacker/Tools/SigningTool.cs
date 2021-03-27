using System;
using System.IO;
using IcyLight.Distribution.Impl;
using NSec.Cryptography;

namespace IcyLightPacker.Tools
{
    internal sealed class SigningTool : IDisposable
    {
        private readonly Ed25519 _algorithm = SignatureAlgorithm.Ed25519;
        private readonly Key _privateKey;
        
        public SigningTool(string ed25519KeyPath)
        {
            if (!File.Exists(ed25519KeyPath))
                throw new ArgumentException("ed25519 Private Key file does not exist", nameof(ed25519KeyPath));

            var key = Ed25519Key.FromFile(ed25519KeyPath);
            _privateKey = Key.Import(_algorithm, key.ToBytes(), KeyBlobFormat.PkixPrivateKey);
        }

        public string Sign(in byte[] data)
        {
            var signature = _algorithm.Sign(_privateKey, data);
            return Convert.ToBase64String(signature);
        }

        public void Dispose()
        {
            _privateKey?.Dispose();
        }
    }
}