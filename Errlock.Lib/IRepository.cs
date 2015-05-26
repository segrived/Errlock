using System;
using System.Collections.Generic;

namespace Errlock.Lib
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Добавляет элемент, если элемента с таким ID ещё не существует, иначе обновляет его
        /// </summary>
        /// <param name="item">Добавляемый или обновляемый элемент</param>
        void InsertOrUpdate(T item);

        /// <summary>
        /// Удаляет элемент
        /// </summary>
        /// <param name="item"></param>
        void Delete(T item);

        /// <summary>
        /// Перечисляет все доступные элементы
        /// </summary>
        /// <returns>Перечисление всех существующих элементов</returns>
        IEnumerable<T> EnumerateAll();

        /// <summary>
        /// Получает элемент по ID
        /// </summary>
        /// <param name="id">ID элемента</param>
        /// <returns>Элемент, найденный по указанному ID</returns>
        T GetItemById(Guid id);

        /// <summary>
        /// Проверяет элемент на существование. Возвращает True если элемент существует; 
        /// иначе - False
        /// </summary>
        /// <param name="id">ID искомого элемента</param>
        /// <returns>True - если элемент существует; иначе - False</returns>
        bool Exists(Guid id);
    }
}
