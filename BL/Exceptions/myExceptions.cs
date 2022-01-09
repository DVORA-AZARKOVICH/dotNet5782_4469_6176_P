using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    internal class myExceptions : Exception
    {
        [Serializable]
        internal class itemNotFoundExceptions : Exception
        {

            public itemNotFoundExceptions()
            {
            }

            public itemNotFoundExceptions(string message) : base(message)
            {
            }

            public itemNotFoundExceptions(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected itemNotFoundExceptions(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        [Serializable]
        internal class itemAllreadyExistException : Exception
        {
            public itemAllreadyExistException()
            {
            }

            public itemAllreadyExistException(string message) : base(message)
            {
            }

            public itemAllreadyExistException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected itemAllreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
