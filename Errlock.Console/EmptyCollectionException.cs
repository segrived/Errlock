using System;
using System.Runtime.Serialization;

namespace Errlock.Console
{
    [Serializable]
    public class EmptyCollectionException : Exception
    {
        public EmptyCollectionException() { }
        public EmptyCollectionException(string message) : base(message) { }
        public EmptyCollectionException(string message, Exception inner) : base(message, inner) { }

        protected EmptyCollectionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}