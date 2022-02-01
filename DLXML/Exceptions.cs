using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DLXML
{
    class Exceptions
    {
        [Serializable]
        internal class XMLFileLoadCreateException : Exception
        {
            public XMLFileLoadCreateException()
            {
            }

            public XMLFileLoadCreateException(string message) : base(message)
            {
            }

            public XMLFileLoadCreateException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected XMLFileLoadCreateException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            protected XMLFileLoadCreateException( string filePath, StreamingContext context, SerializationInfo info) : base(info, context)
            {
            }
        }
    }
    [Serializable]
    internal class IdExistException : Exception
    {
        public IdExistException()
        {
        }

        public IdExistException(string message) : base(message)
        {
        }

        public IdExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IdExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected IdExistException(string filePath, StreamingContext context, SerializationInfo info) : base(info, context)
        {
        }
    }

    [Serializable]
    internal class IDdNotFoundExeption : Exception
    {
        public IDdNotFoundExeption()
        {
        }

        public IDdNotFoundExeption(string message) : base(message)
        {
        }

        public IDdNotFoundExeption(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IDdNotFoundExeption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

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
