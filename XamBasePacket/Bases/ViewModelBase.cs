using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Navigation;

namespace XamBasePacket.Bases
{

    public abstract class ViewModelBase : BindableBase, INavigationAware
    {
        private bool _isBusy;
        private string _errorText;

        protected ViewModelBase()
        {
        }

        public virtual bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value, OnIsBusyChanged);
        }

        protected virtual void OnIsBusyChanged()
        {

        }

        public string ErrorText
        {
            get => _errorText;
            set => SetProperty(ref _errorText, value);
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
            ClearErrors();
            Task.Run(() => Initialize(parameters).ConfigureAwait(false));
        }


        protected virtual Task Initialize(INavigationParameters parameters = null)
        {
            return Task.FromResult(true);
        }

        protected virtual void ClearErrors()
        {
            if (!string.IsNullOrEmpty(ErrorText))
                ErrorText = null;
        }

        public virtual void UnauthorizedApiCall()
        {
        }

        public virtual void DisplayError(string error)
        {
            ErrorText = error;
        }

    }
}
