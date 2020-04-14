using System;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Navigation;

namespace XamBasePacket.Bases
{

    public  abstract class ViewModelBase : BindableBase, INavigatedAware, IInitialize, IInitializeAsync
    {


        #region properties
        private bool _isBusy;
        public virtual bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value, OnIsBusyChanged);
        }

        private string _errorText;
        public string ErrorText
        {
            get => _errorText;
            set => SetProperty(ref _errorText, value);
        }


        private Exception _error;
        public Exception Error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }

        #endregion

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }


        public virtual void UnauthorizedApiCall()
        {
        }

        public virtual void DisplayError(Exception ex, string errorMessage = null, bool displayError = true)
        {
            Error = ex;
            if (displayError)
                ErrorText = errorMessage ?? ex?.Message;
        }

        public virtual void Initialize(INavigationParameters parameters)
        {
            ClearErrors();
        }

        public virtual Task InitializeAsync(INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }


        #region protected
        protected virtual void OnIsBusyChanged()
        {

        }



        protected virtual void ClearErrors()
        {
            if (!string.IsNullOrEmpty(ErrorText))
                ErrorText = null;
            Error = null;
        }


        #endregion

    }
}
