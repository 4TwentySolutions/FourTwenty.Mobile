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

        public static Task<FileItem> PickPhoto(PickMediaOptions options)
        {
            return PickPhoto(true, options);
        }
        /// <summary>
        /// Pick photo from gallery
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <param name="options"></param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> PickPhoto(bool withData, PickMediaOptions options)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                throw new InvalidOperationException("Photo picking is not supported");
            }

            var file = await CrossMedia.Current.PickPhotoAsync(options);
            return HandleMediaFile(file, withData);
        }


        public static Task<FileItem> TakePhoto()
        {
            return TakePhoto(true);
        }

        public static Task<FileItem> TakePhoto(bool withData)
        {
            return TakePhoto(withData, null);
        }

        public static Task<FileItem> TakePhoto(StoreCameraMediaOptions options)
        {
            return TakePhoto(true, options);
        }
        /// <summary>
        /// Take photo from camera
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <param name="options"></param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> TakePhoto(bool withData, StoreCameraMediaOptions options)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                throw new InvalidOperationException("Photo taking is not supported");
            }

            var file = await CrossMedia.Current.TakePhotoAsync(options ?? new StoreCameraMediaOptions());
            return HandleMediaFile(file, withData);
        }
        public static Task<FileItem> TakeVideo()
        {
            return TakeVideo(true);
        }

        public static Task<FileItem> TakeVideo(bool withData)
        {
            return TakeVideo(withData, null);
        }

        public static Task<FileItem> TakeVideo(StoreVideoOptions options)
        {
            return TakeVideo(true, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <param name="options"></param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> TakeVideo(bool withData, StoreVideoOptions options)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsTakeVideoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                throw new InvalidOperationException("Video taking is not supported");
            }

            var file = await CrossMedia.Current.TakeVideoAsync(options ?? new StoreVideoOptions());
            return HandleMediaFile(file, withData);
        }
        public static Task<FileItem> PickVideo()
        {
            return PickVideo(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="withData">if false data will be null</param>
        /// <returns>Return path and data in bytes</returns>
        public static async Task<FileItem> PickVideo(bool withData)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsTakeVideoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                throw new InvalidOperationException("Video taking is not supported");
            }

            var file = await CrossMedia.Current.PickVideoAsync();
            return HandleMediaFile(file, withData);
        }

        private static FileItem HandleMediaFile(MediaFile file, bool withData)
        {
            FileItem result = new FileItem();
            if (file == null)
                return null;
            result.Url = file.Path;
            result.FileName = Path.GetFileName(file.Path);
            if (withData)
                result.Data = file.GetStream();
            file.Dispose();
            return result;
        }

    }
}
