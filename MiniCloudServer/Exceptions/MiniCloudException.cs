using System;
using System.Runtime.Serialization;

namespace MiniCloudServer.Exceptions
{
    public class MiniCloudException : Exception
    {
        public MiniCloudException()
        {
        }

        public MiniCloudException(string message) : base(message)
        {
        }

        public MiniCloudException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MiniCloudException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
