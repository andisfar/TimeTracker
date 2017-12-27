using System;
using System.Runtime.Serialization;

namespace TimeTrackerDataAccessLayer
{
    [Serializable]
    internal class InvalidConnectionStringProvidedException : Exception
    {
        public InvalidConnectionStringProvidedException()
        {
        }

        public InvalidConnectionStringProvidedException(string message) : base(message)
        {
        }

        public InvalidConnectionStringProvidedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidConnectionStringProvidedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}