using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DLObject.Exceptions
{
    internal class Exceptions
    {
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
        
    }
}
