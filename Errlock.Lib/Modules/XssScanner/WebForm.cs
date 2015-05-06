using System;
using Errlock.Lib.SmartWebRequest;

namespace Errlock.Lib.Modules.XssScanner
{
    public class WebForm
    {
        protected bool Equals(WebForm other)
        {
            return string.Equals(Action, other.Action) && RequestType == other.RequestType;
        }

        public override int GetHashCode()
        {
            unchecked {
                return ((Action != null ? Action.GetHashCode() : 0) * 397) ^ (int)RequestType;
            }
        }

        public string Action { get; set; }
        public RequestType RequestType { get; set; }

        public WebForm(string action, RequestType requestType)
        {
            this.Action = action;
            this.RequestType = requestType;
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
            return string.Format("{0} {1}", RequestType.GetDescription(), Action);
        }
    }
}
