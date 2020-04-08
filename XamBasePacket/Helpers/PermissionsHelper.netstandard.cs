using System;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Services;
using XamBasePacket.Resources;

namespace XamBasePacket.Helpers
{
    public static class PermissionsHelper
    {
        public class PermissionParameters
        {
            public Permission Permission { get; set; }
            public IPageDialogService PageDialogService { get; set; }
            public string RationaleTitle { get; set; }
            public string RationaleMessage { get; set; }
            public string ErrorTitle { get; set; }
            public string ErrorMessage { get; set; }
        }

        public static async Task<bool> PermissionsRequest<T>(PermissionParameters parameters) where T : BasePermission, new()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<T>();
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(parameters.Permission))
                    {
                        await parameters.PageDialogService.DisplayAlertAsync(parameters.RationaleTitle, parameters.RationaleMessage, BaseResource.Ok);
                    }

                    status = await CrossPermissions.Current.RequestPermissionAsync<T>();
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }

                if (status != PermissionStatus.Unknown)
                {
                    await parameters.PageDialogService.DisplayAlertAsync(parameters.ErrorTitle, parameters.ErrorMessage, BaseResource.Ok);
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return false;
        }
        [Obsolete("Use PermissionsRequest<T> instead")]
        public static async Task<bool> PermissionsRequest(PermissionParameters parameters)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(parameters.Permission);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(parameters.Permission))
                    {
                        await parameters.PageDialogService.DisplayAlertAsync(parameters.RationaleTitle, parameters.RationaleMessage, BaseResource.Ok);
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(parameters.Permission);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(parameters.Permission))
                        status = results[parameters.Permission];
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }

                if (status != PermissionStatus.Unknown)
                {
                    await parameters.PageDialogService.DisplayAlertAsync(parameters.ErrorTitle, parameters.ErrorMessage, BaseResource.Ok);
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return false;
        }
    }
}
