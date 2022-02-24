using System;
using System.Runtime.Serialization;

namespace Dal
{
    [Serializable]
    internal class XmlFailedToLoadCreatException : Exception
    {
        private string filePath;
        private string v;
        private Exception ex;

        public XmlFailedToLoadCreatException()
        {
        }

        public XmlFailedToLoadCreatException(string message) : base(message)
        {
        }

        public XmlFailedToLoadCreatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public XmlFailedToLoadCreatException(string filePath, string v, Exception ex)
        {
            this.filePath = filePath;
            this.v = v;
            this.ex = ex;
        }

        protected XmlFailedToLoadCreatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}