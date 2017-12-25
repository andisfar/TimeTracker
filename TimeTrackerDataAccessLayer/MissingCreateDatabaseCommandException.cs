using System;
using System.Runtime.Serialization;

namespace TimeTrackerDataAccessLayer
{
    [Serializable]
    internal class MissingCreateDatabaseCommandException : Exception
    {
        public MissingCreateDatabaseCommandException()
        {
        }

        public MissingCreateDatabaseCommandException(string message) : base(message)
        {
        }

        public MissingCreateDatabaseCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingCreateDatabaseCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}