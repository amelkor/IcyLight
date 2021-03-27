using System;
using NSec.Cryptography;

namespace IcyLight.Distribution.Impl
{
    public sealed class SignatureTool
    {
        private readonly Ed25519 _algorithm = SignatureAlgorithm.Ed25519;
        private readonly PublicKey _publicKey;
        
        public SignatureTool(Ed25519Key key)
        {
            _publicKey = PublicKey.Import(_algorithm, key.ToBytes(), KeyBlobFormat.PkixPublicKey);
        }
        
        public bool Verify(string signature, in ReadOnlySpan<byte> data)
        {
            return _algorithm.Verify(_publicKey, data, Convert.FromBase64String(signature));
        }
    }
}