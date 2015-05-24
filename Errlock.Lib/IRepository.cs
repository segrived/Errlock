using System;
using System.Collections.Generic;

namespace Errlock.Lib
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Добавляет элемент
        /// </summary>
        /// <param name="item"></param>
        void Insert(T item);

        /// <summary>
        /// Удаляет элемент
        /// </summary>
        /// <param name="item"></param>
        void Delete(T item);

        /// <summary>
        /// Перечисляет все доступные элементы
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> EnumerateAll();

        /// <summary>
        /// Получает элемент по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetItemById(Guid id);

        /// <summary>
        /// Проверяет элемент на существование
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(Guid id);
    }
}
