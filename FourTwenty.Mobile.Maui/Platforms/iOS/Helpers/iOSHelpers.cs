using UIKit;

namespace FourTwenty.Mobile.Maui.Helpers
{
    public static class iOSHelpers
    {
        public static UIViewController? GetVisibleViewController()
        {
            try
            {
                var rootController = UIApplication.SharedApplication.KeyWindow?.RootViewController;

                switch (rootController?.PresentedViewController)
                {
                    case null:
                        return rootController;
                    case UINavigationController controller:
                        return controller.VisibleViewController;
                    case UITabBarController barController:
                        return barController.SelectedViewController;
                    default:
                        return rootController.PresentedViewController;
                }
            }
            catch (Exception)
            {
                return UIApplication.SharedApplication.KeyWindow?.RootViewController;
            }
        }

        public static UIBarButtonSystemItem ToUiReturnKeyType(this ReturnType returnType)
        {
            switch (returnType)
            {
                case ReturnType.Go:
                    return UIBarButtonSystemItem.Add;
                case ReturnType.Next:
                    return UIBarButtonSystemItem.Action;
                case ReturnType.Send:
                    return UIBarButtonSystemItem.Save;
                case ReturnType.Search:
                    return UIBarButtonSystemItem.Search;
                case ReturnType.Done:
                    return UIBarButtonSystemItem.Done;
                case ReturnType.Default:
                    return UIBarButtonSystemItem.Done;
                default:
                    throw new System.NotImplementedException($"ReturnType {returnType} not supported");
            }
        }
    }
}
