using System;
using Errlock.Lib;

namespace Errlock.ViewModels
{
    public abstract class CloseableViewModel : Bindable
    {
        public event EventHandler ClosingRequest;

        protected void OnClosingRequest()
        {
            if (this.ClosingRequest != null) {
                this.ClosingRequest(this, EventArgs.Empty);
            }
        }
    }
}