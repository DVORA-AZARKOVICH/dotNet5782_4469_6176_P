using System;
using System.Runtime.Serialization;

namespace DALFasade.DALApi
{
    [Serializable]
    internal class DalConfigException : Exception
    {
        public DalConfigException()
        {
        }

        public DalConfigException(string message) : base(message)
        {
        }

        public DalConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DalConfigException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}