using System;
using System.IO;
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
        public async void WebApiStreamTypeTesting()
        {
            var service = new WebApiTypesService();
            var data = await service.GetFile();
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
    }
}
