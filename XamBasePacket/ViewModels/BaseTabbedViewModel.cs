using System;
using Prism;
using XamBasePacket.Bases;
using XamBasePacket.Bases.Validation;

namespace XamBasePacket.ViewModels
{
    public class BaseTabbedViewModel : ValidationViewModelBase, IActiveAware
    {
        public BaseTabbedViewModel()
        {

        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value, RaiseIsActiveChanged);
        }

        public event EventHandler IsActiveChanged;

        protected virtual void RaiseIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
