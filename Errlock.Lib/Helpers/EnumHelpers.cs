using System;
using System.Collections.Generic;
using System.Linq;

namespace Errlock.Lib.Helpers
{
    /// <summary>
    /// Функции-хелперы для перечислений (Enum)
    /// </summary>
    public static class EnumHelpers
    {
        /// <summary>
        /// Преобразовывает перечисление (Enum) в словарь, где ключ это сам элемент 
        /// перечисления, а значение - его описание (используется атрибут Description)
        /// </summary>
        /// <typeparam name="T">Тип перечисления</typeparam>
        /// <returns>Словарь, где ключ - сам элемент, а значение - его описание</returns>
        public static Dictionary<T, string> EnumToDictionary<T>()
        {
            var enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum)) {
                throw new ArgumentException("Неверный тип значения");
            }
            var resultDict = Enum.GetValues(enumType)
                       .Cast<T>()
                       .ToDictionary(v => v, v => (v as Enum).GetDescription());
            return resultDict;
        }
    }
}