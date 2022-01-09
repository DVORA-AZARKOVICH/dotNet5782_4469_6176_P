using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    [Serializable]
    internal class ExceptionExceptionExceedingPossibleQuantity : Exception
    {
        public ExceptionExceptionExceedingPossibleQuantity()
        {
        }

        public ExceptionExceptionExceedingPossibleQuantity(string message) : base(message)
        {
        }

        public ExceptionExceptionExceedingPossibleQuantity(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExceptionExceptionExceedingPossibleQuantity(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
