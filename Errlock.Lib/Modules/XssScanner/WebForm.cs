using System;
using System.Collections.Generic;
using Errlock.Lib.SmartWebRequest;

namespace Errlock.Lib.Modules.XssScanner
{
    public class WebForm : IEquatable<WebForm>
    {
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

        public string Action { get; set; }
        public RequestMethod RequestMethod { get; set; }
        public List<WebFormElement> WebFormElements { get; set; }

        public WebForm(string action, RequestMethod requestMethod)
        {
            this.Action = action;
            this.RequestMethod = requestMethod;
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
