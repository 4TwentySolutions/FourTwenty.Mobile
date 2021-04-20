using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace XamBasePacket.Helpers
{
    [Obsolete("User Xamarin Essentials directly")]
    public static class ImagePickerHelper
    {

        public class FileItem
        {
            public FileItem()
            {

            }
            public FileItem(string fileName) : this(fileName, fileName)
            {
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


        public static Task<FileItem> PickPhoto()
        {
            return PickPhoto(true);
        }

        public static Task<FileItem> PickPhoto(bool withData)
        {
            return PickPhoto(withData, null);
        }

        public static Task<FileItem> PickPhoto(MediaPickerOptions options)
        {
            return PickPhoto(true, options);
        }
        /// <summary>
        /// Pick photo from gallery
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <param name="options"></param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> PickPhoto(bool withData, MediaPickerOptions options)
        {
            var file = await
                MediaPicker.PickPhotoAsync(options);
            return await HandleMediaFile(file, withData);
        }


        public static Task<FileItem> TakePhoto()
        {
            return TakePhoto(true);
        }

        public static Task<FileItem> TakePhoto(bool withData)
        {
            return TakePhoto(withData, null);
        }

        public static Task<FileItem> TakePhoto(MediaPickerOptions options)
        {
            return TakePhoto(true, options);
        }
        /// <summary>
        /// Take photo from camera
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <param name="options"></param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> TakePhoto(bool withData, Xamarin.Essentials.MediaPickerOptions options)
        {

            if (!MediaPicker.IsCaptureSupported)
            {
                throw new InvalidOperationException("Photo taking is not supported");
            }

            var file = await MediaPicker.CapturePhotoAsync(options ?? new Xamarin.Essentials.MediaPickerOptions());
            return await HandleMediaFile(file, withData);
        }
        public static Task<FileItem> TakeVideo()
        {
            return TakeVideo(true);
        }

        public static Task<FileItem> TakeVideo(bool withData)
        {
            return TakeVideo(withData, null);
        }

        public static Task<FileItem> TakeVideo(MediaPickerOptions options)
        {
            return TakeVideo(true, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <param name="options"></param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> TakeVideo(bool withData, Xamarin.Essentials.MediaPickerOptions options)
        {
            if (!MediaPicker.IsCaptureSupported)
            {
                throw new InvalidOperationException("Video taking is not supported");
            }

            var file = await MediaPicker.CaptureVideoAsync(options ?? new MediaPickerOptions());
            return await HandleMediaFile(file, withData);
        }
        public static Task<FileItem> PickVideo(MediaPickerOptions options)
        {
            return PickVideo(true, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> PickVideo(bool withData, MediaPickerOptions options)
        {
            var file = await MediaPicker.PickVideoAsync(options ?? new MediaPickerOptions());
            return await HandleMediaFile(file, withData);
        }

        private static async Task<FileItem> HandleMediaFile(FileResult file, bool withData)
        {
            FileItem result = new FileItem();
            if (file == null)
                return null;
            result.Url = file.FullPath;
            result.FileName = Path.GetFileName(file.FullPath);
            if (withData)
                result.Data = await file.OpenReadAsync();
            return result;
        }

    }
}
