using System;

namespace Errlock.Lib.Sessions
{
    public class SessionEventArgs : EventArgs
    {
        public SessionEventType EventType { get; private set; }

        public SessionEventArgs(SessionEventType eventType)
        {
            this.EventType = eventType;
        }
    }
}