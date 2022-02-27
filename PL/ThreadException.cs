using System;
using System.Runtime.Serialization;

namespace PL
{
    [Serializable]
    internal class ThreadException : Exception
    {
        public ThreadException()
        {
        }

        public ThreadException(string message) : base(message)
        {
        }

        public ThreadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ThreadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}