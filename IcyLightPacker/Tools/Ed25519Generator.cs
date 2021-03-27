using System;
using IcyLight.Distribution.Impl;
using NSec.Cryptography;

namespace IcyLightPacker.Tools
{
    public static class Ed25519KeyGenerator
    {
        public static (Ed25519Key privateKey, Ed25519Key publicKey) Create()
        {
            using var key = Key.Create(SignatureAlgorithm.Ed25519, new KeyCreationParameters() {ExportPolicy = KeyExportPolicies.AllowPlaintextExport});
            return (new Ed25519Key(
                nameof(SignatureAlgorithm.Ed25519),
                "private",
                Convert.ToBase64String(key.Export(KeyBlobFormat.PkixPrivateKey))
                ),
                new Ed25519Key(
                nameof(SignatureAlgorithm.Ed25519),
                "public",
                Convert.ToBase64String(key.Export(KeyBlobFormat.PkixPublicKey))
            ));
        }
    }
}