using System.Collections.Generic;

namespace Errlock.Lib.Modules.XssScanner
{
    public enum ElementType { Input, Select, TextArea, Button }

    public class WebFormElement
    {
        public string Name { get; set; }
        public ElementType Type { get; set; }
        public List<string> AvailableValues { get; set; }

        public WebFormElement(string name, ElementType type)
        {
            Name = name;
            Type = type;
        }

        internal WebFormElement(string name, ElementType type, List<string> availableValues)
        {
            this.Name = name;
            this.Type = type;
            this.AvailableValues = availableValues;
        }
    }
}
