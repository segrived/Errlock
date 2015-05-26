using System.Collections.Generic;

namespace Errlock.Lib.Modules.XssScanner
{
    public enum ElementType { Input, Select, TextArea, Button }

    public class WebFormElement
    {
        public string Name { get; set; }
        public ElementType Type { get; set; }
        public List<string> AvailableValues { get; set; }
        public bool IsInjectionAllowed { get; set; }

        internal WebFormElement(string name, ElementType type)
        {
            Name = name;
            Type = type;
        }

        internal WebFormElement(string name, ElementType type, bool isInjectionAllowed, 
            List<string> availableValues = null)
        {
            this.Name = name;
            this.Type = type;
            this.AvailableValues = availableValues;
            this.IsInjectionAllowed = isInjectionAllowed;
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", this.Name, this.AvailableValues.PickRandom());
        }
    }
}
