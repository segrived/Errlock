using System;

namespace Errlock.Lib.Helpers
{
    public static class GuidHelpers
    {
        public static bool IsValidGuid(string guid)
        {
            Guid guidOutput;
            bool isValid = Guid.TryParse(guid, out guidOutput);
            return isValid;
        }
    }
}