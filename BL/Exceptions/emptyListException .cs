using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    [Serializable]
    internal class emptyListException : Exception
    {
        public emptyListException()
        {
        }

        public emptyListException(string message) : base(message)
        {
        }

        public emptyListException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected emptyListException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
