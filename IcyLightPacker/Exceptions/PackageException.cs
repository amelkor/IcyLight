using System;
using System.Runtime.Serialization;

namespace IcyLightPacker.Exceptions
{
    [Serializable]
    public class PackageException : Exception
    {
        public PackageException()
        {
        }

        public PackageException(string message) : base(message)
        {
        }

        public PackageException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PackageException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}