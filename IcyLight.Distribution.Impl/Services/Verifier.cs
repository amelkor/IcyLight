using System.IO;
using Force.Crc32;
using IcyLight.Core;
using IcyLight.Distribution.Services;

namespace IcyLight.Distribution.Impl.Services
{
    public class Verifier : IVerifier
    {
        private readonly SignatureTool _signatureTool;
        
        public Verifier(Ed25519Key publicKey)
        {
            _signatureTool = new SignatureTool(publicKey);
        }
        
        public bool VerifyFile(DistributionFile file, FileInfo fileInfo)
        {
            var bytes = File.ReadAllBytes(fileInfo.FullName);
            
            return 
                Crc32CAlgorithm.Compute(bytes) == file.Hash &&
                _signatureTool.Verify(file.Signature, bytes);
        }
    }
}