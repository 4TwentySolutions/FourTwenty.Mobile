namespace FourTwenty.Mobile.Maui.Interfaces
{
    public interface IMainApplication
    {
        Task<INavigationResult> ResolveInitialNavigation();
    }
}
