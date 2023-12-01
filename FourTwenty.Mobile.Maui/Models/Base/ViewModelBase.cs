namespace FourTwenty.Mobile.Maui.Models.Base
{
    public abstract class ViewModelBase : BindableBase, INavigatedAware, IInitialize, IInitializeAsync
    {

        #region properties
        private bool _isBusy;
        public virtual bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value, OnIsBusyChanged);
        }

        #endregion

        public virtual void OnNavigatedFrom(INavigationParameters parameters) { }

        public virtual void OnNavigatedTo(INavigationParameters parameters) { }

        public virtual void Initialize(INavigationParameters parameters) { }

        public virtual Task InitializeAsync(INavigationParameters parameters) { return Task.CompletedTask; }

        #region protected

        public virtual void ShowLoader() => IsBusy = true;
        public virtual void HideLoader() => IsBusy = false;

        protected virtual void OnIsBusyChanged() { }

        #endregion
    }
}
