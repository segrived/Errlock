using Errlock.Lib.RequestWrapper;

namespace Errlock.Lib.Modules.PasswordCrackerModule
{
    public class PasswordCrackerConfig : ModuleConfig
    {
        /// <summary>
        /// Логин, для которого требуется подобрать пароль
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// URL, на который требудется отправлять запрос
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// Параметры запроса
        /// </summary>
        public string RequestParameters { get; set; }

        /// <summary>
        /// Действие ресурса при вводе неправильного пароля
        /// </summary>
        public InvalidPasswordAction InvalidPasswordAction { get; set; }

        /// <summary>
        /// Метод запроса
        /// </summary>
        public RequestMethod RequestMethod { get; set; }

        /// <summary>
        /// Количество проверяемых паролей
        /// </summary>
        public int PasswordsCount { get; set; }

        /// <summary>
        /// Останавливать после первого совпадения
        /// </summary>
        public bool StopAfterFirstMatch { get; set; }

        /// <summary>
        /// Использовать эвристику
        /// </summary>
        public bool UseHeuristic { get; set; }
    }
}