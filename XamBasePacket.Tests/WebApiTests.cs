using System;
using System.IO;
using System.Threading.Tasks;
using XamBasePacket.Bases;
using XamBasePacket.Helpers;
using XamBasePacket.Tests.Services;
using XamBasePacket.ViewModels;
using Xunit;

namespace XamBasePacket.Tests
{
    public class WebApiTests
    {
        [Fact]
        public async Task WebApiStreamTypeTesting()
        {
            var service = new WebApiTypesService();
            var data = await service.GetFile();
            using (var memStream = new MemoryStream())
            {
                await data.Content.CopyToAsync(memStream);
            }

            Assert.True(data.IsSuccess);
            Assert.IsAssignableFrom<Stream>(data.Content);
        }

        [Fact]
        public async void WebApiMemoryStreamTypeTesting()
        {
            var service = new WebApiTypesService();
            var data = await service.GetFileMemory();
            Assert.True(data.IsSuccess);
            Assert.IsAssignableFrom<MemoryStream>(data.Content);
        }

        [Fact]
        public async void WebApiJsonModelTypeTesting()
        {
            var service = new WebApiTypesService();
            var data = await service.GetModelData();
            Assert.True(data.IsSuccess);
            Assert.IsAssignableFrom<WebApiTypesService.SomeModel>(data.Content);
        }


        [Fact]
        public async void WrapTaskTest()
        {
            var service = new WebApiTypesService();
            var viewModel = new BaseTabbedViewModel();
            var data = await service.GetModelData().WrapTaskWithApiResponse(viewModel);
            Assert.True(data.IsSuccess);
            Assert.IsAssignableFrom<WebApiTypesService.SomeModel>(data.Content);
        }

        [Fact]
        public async void WrapTaskDeepTest()
        {
            var service = new WebApiTypesService();
            var viewModel = new BaseTabbedViewModel();
            var data = await service.GetModelData().WrapTaskWithApiResponse(viewModel).WrapTaskWithLoading(viewModel).WrapWithExceptionHandling(viewModel);
            Assert.True(data.IsSuccess);
            Assert.IsAssignableFrom<WebApiTypesService.SomeModel>(data.Content);
        }

        [Fact]
        public async void WrapTaskDummyTest()
        {
            var service = new WebApiTypesService();
            var viewModel = new BaseTabbedViewModel();
            await service.Dummy().WrapTaskWithLoading(viewModel).WrapWithExceptionHandling(viewModel);
        }

        [Fact]
        public async Task WebApiStreamTypeGoogleTesting()
        {
            var service = new WebApiTypesService();
            var data = await service.GetGoogleStream();
            using (var memStream = new MemoryStream())
            {
                await data.Content.CopyToAsync(memStream);
                
                Assert.True(memStream.Length > 0);
            }

            Assert.True(data.IsSuccess);
            Assert.IsAssignableFrom<Stream>(data.Content);
        }
    }
}
