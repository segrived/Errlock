using System;
using System.Collections.Generic;
using System.Linq;

namespace Errlock.Lib.Helpers
{
    public static class EnumHelpers
    {
        public static Dictionary<T, string> EnumToDictionary<T>()
        {
            var enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum)) {
                throw new ArgumentException("Неверный тип значения");
            }
            return Enum.GetValues(enumType)
                       .Cast<T>()
                       .ToDictionary(v => v, v => (v as Enum).GetDescription());
        }
    }
}