using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CsQuery;
using Errlock.Lib.Helpers;
using Errlock.Lib.RequestWrapper;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.XssScanner
{
    public class WebForm : IEquatable<WebForm>
    {
        private const string XssVector = "<script>alert('1');</script>";

        public bool Equals(WebForm other)
        {
            return string.Equals(Action, other.Action) && RequestMethod == other.RequestMethod;
        }

        public override int GetHashCode()
        {
            unchecked {
                return ((Action != null ? Action.GetHashCode() : 0) * 397) ^ (int)RequestMethod;
            }
        }

        private IDomObject _formDomObject;
        private string Action { get; set; }
        private RequestMethod RequestMethod { get; set; }
        private IEnumerable<WebFormElement> WebFormElements { get; set; }

        public WebForm(IDomObject form, Session session)
        {
            this._formDomObject = form;
            this.Action = form.GetAttribute("action").MakeUrlAbsolute(session.Url);
            string method = form.GetAttribute("method");
            this.RequestMethod = WebHelpers.ToRequestMethod(method);
            this.WebFormElements = this.ParseFormElements();
        }

        private IEnumerable<WebFormElement> ParseFormElements()
        {
            var formCq = this._formDomObject.Cq();
            var inputElements = formCq["input,select,textarea"].Select(ParseInputElement);
            return inputElements;
        }

        /// <summary>
        /// Выполняем парсинг элемент формы и возвращает экземпляр класса WebFormElement
        /// </summary>
        /// <param name="input">Исходный элемент типа IDomObject</param>
        /// <returns>Экземпляр объекта WebFormElement, представляющий элемент формы</returns>
        private WebFormElement ParseInputElement(IDomObject input)
        {
            var name = input.GetAttribute("name");
            ElementType elementType;
            switch (input.NodeName.ToLower()) {
                case "input":
                    elementType = ElementType.Input;
                    return new WebFormElement(name, elementType, true, new List<string> { "test" });
                case "textarea":
                    elementType = ElementType.TextArea;
                    return new WebFormElement(name, elementType, true, new List<string> { "test" });
                case "select":
                    elementType = ElementType.Select;
                    var availableValues =
                        input.Cq()["option"].Select(x => x.GetAttribute("value")).ToList();
                    return new WebFormElement(name, ElementType.Select, true, availableValues);
                default:
                    throw new ArgumentException("Неподдерживаемый тип элемента");
            }
        }

        public bool HasInjection(string input)
        {
            return input.Contains(XssVector);
        }

        /// <summary>
        /// Генерирует строку параметров на основе данных формы
        /// </summary>
        /// <returns></returns>
        private string ToQueryString(NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (string key in nvc.AllKeys) {
                var strings = nvc.GetValues(key);
                if (strings == null) {
                    continue;
                }
                foreach (string value in strings) {
                    if (!first) {
                        sb.Append("&");
                    }
                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key),
                        Uri.EscapeDataString(value));
                    first = false;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Генерирует полный URL, на основе данных формы
        /// </summary>
        /// <returns></returns>
        public string GetQuery()
        {
            var collection = new NameValueCollection();
            foreach (var element in this.WebFormElements) {
                string value = String.Empty;
                if (element.IsInjectionAllowed) {
                    // injection string
                    value = XssVector;
                } else {
                    value = element.AvailableValues.PickRandom();
                }
                collection.Add(element.Name, value);
            }
            var builder = new UriBuilder(this.Action) {
                Query = ToQueryString(collection)
            };
            return builder.Uri.AbsoluteUri;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != this.GetType()) {
                return false;
            }
            return Equals((WebForm)obj);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", RequestMethod.GetDescription(), Action);
        }
    }
}
