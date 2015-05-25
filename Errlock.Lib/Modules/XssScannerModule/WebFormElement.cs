using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace Errlock.Lib.Modules.XssScanner
{
    public enum ElementType { Input, Select, TextArea, Button }

    public class WebFormElement
    {
        public string Name { get; set; }
        public ElementType Type { get; set; }
        public List<string> AvailableValues { get; set; }
        public bool IsInextionAllowed { get; set; }

        internal WebFormElement(string name, ElementType type)
        {
            Name = name;
            Type = type;
        }

        internal WebFormElement(string name, ElementType type, List<string> availableValues, bool isInjectionAllowed)
        {
            this.Name = name;
            this.Type = type;
            this.AvailableValues = availableValues;
            this.IsInextionAllowed = isInjectionAllowed;
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", this.Name, this.AvailableValues.PickRandom());
        }
    }
}
