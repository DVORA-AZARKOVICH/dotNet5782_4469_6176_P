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
}
