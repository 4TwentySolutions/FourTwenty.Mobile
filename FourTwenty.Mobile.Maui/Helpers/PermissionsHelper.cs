using System;
using System.Threading.Tasks;
using FourTwenty.Mobile.Maui.Resources;


namespace FourTwenty.Mobile.Maui.Helpers
{
    public static class PermissionsHelper
    {
        public class PermissionParameters
        {
            public IPageDialogService PageDialogService { get; set; }
            public string? ErrorTitle { get; set; }
            public string? ErrorMessage { get; set; }
            public bool ShowError { get; set; } = true;
        }

        public class PermissionResult
        {
            public PermissionResult(PermissionStatus status)
            {
                Status = status;
            }

            public PermissionResult(Exception error)
            {
                Error = error;
            }
            public bool Result => Status == PermissionStatus.Granted && Error == null;
            public PermissionStatus Status { get; set; }
            public Exception? Error { get; set; }

            public static implicit operator bool(PermissionResult d) => d.Result;

        }

        public static async Task<PermissionResult> PermissionsRequest<T>(PermissionParameters parameters) where T : Permissions.BasePermission, new()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<T>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<T>();
                }

                if (status == PermissionStatus.Granted)
                    return new PermissionResult(status);


                if (status != PermissionStatus.Unknown && parameters.ShowError)
                {
                    await parameters.PageDialogService.DisplayAlertAsync(parameters.ErrorTitle, parameters.ErrorMessage, BaseResource.Ok);
                }

                return new PermissionResult(status);
            }
            catch (Exception ex)
            {
                return new PermissionResult(ex);
            }
        }
    }
}
