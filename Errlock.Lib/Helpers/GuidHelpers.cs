using System;

namespace Errlock.Lib.Helpers
{
    /// <summary>
    /// Функции-хелперы для экземпляров класса Guid
    /// </summary>
    public static class GuidHelpers
    {
        /// <summary>
        /// Проверяет указанный Guid на правильность
        /// </summary>
        /// <param name="guid">Проверяемый экземпляр класса Guid</param>
        /// <returns>True в случае, если Guid валиден; иначе - False</returns>
        public static bool IsValidGuid(string guid)
        {
            Guid guidOutput;
            bool isValid = Guid.TryParse(guid, out guidOutput);
            return isValid;
        }
    }
}