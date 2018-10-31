using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace XamBasePacket.Helpers
{
    public static class ImagePickerHelper
    {
        public class FileItem
        {
            public FileItem()
            {

            }
            public FileItem(string fileName)
            {
                FileName = fileName;
                Url = fileName;
            }
            public FileItem(string fileName, string url, Stream data = null)
            {
                FileName = fileName;
                Url = url;
                Data = data;
            }

            public string Url { get; set; }
            public string FileName { get; set; }
            public Stream Data { get; set; }
        }


        /// <summary>
        /// Pick photo from gallery
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <param name="options"></param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> PickPhoto(bool withData = true, PickMediaOptions options = null)
        {
            try
            {
                FileItem result = new FileItem();
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    throw new InvalidOperationException("Photo picking is not supported");
                }

                var file = await CrossMedia.Current.PickPhotoAsync(options);
                if (file == null)
                    return null;
                result.Url = file.Path;
                result.FileName = Path.GetFileName(file.Path);
                if (withData)
                    result.Data = file.GetStream();
                file.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Take photo from camera
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <param name="options"></param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> TakePhoto(bool withData = true, StoreCameraMediaOptions options = null)
        {
            try
            {
                FileItem result = new FileItem();
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
                {
                    throw new InvalidOperationException("Photo taking is not supported");
                }

                var file = await CrossMedia.Current.TakePhotoAsync(options ?? new StoreCameraMediaOptions());
                if (file == null)
                    return null;
                result.Url = file.Path;
                result.FileName = Path.GetFileName(file.Path);
                if (withData)
                    result.Data = file.GetStream();
                file.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <param name="options"></param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> TakeVideo(bool withData = true, StoreVideoOptions options = null)
        {
            try
            {
                FileItem result = new FileItem();
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsTakeVideoSupported || !CrossMedia.Current.IsCameraAvailable)
                {
                    throw new InvalidOperationException("Video taking is not supported");
                }

                var file = await CrossMedia.Current.TakeVideoAsync(options ?? new StoreVideoOptions());
                if (file == null)
                    return null;
                result.Url = file.Path;
                result.FileName = Path.GetFileName(file.Path);
                if (withData)
                    result.Data = file.GetStream();
                file.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> PickVideo(bool withData = true)
        {
            try
            {
                FileItem result = new FileItem();
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsTakeVideoSupported || !CrossMedia.Current.IsCameraAvailable)
                {
                    throw new InvalidOperationException("Video taking is not supported");
                }

                var file = await CrossMedia.Current.PickVideoAsync();
                if (file == null)
                    return null;
                result.Url = file.Path;
                result.FileName = Path.GetFileName(file.Path);
                if (withData)
                    result.Data = file.GetStream();
                file.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
