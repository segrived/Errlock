using System;
using System.Runtime.Serialization;

namespace Errlock.Lib.Modules
{
    [Serializable]
    public class ModuleDatabaseFileException : Exception
    {
        public ModuleDatabaseFileException() { }
        public ModuleDatabaseFileException(string message) : base(message) { }
        public ModuleDatabaseFileException(string message, Exception inner) : base(message, inner) { }

        protected ModuleDatabaseFileException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }
}