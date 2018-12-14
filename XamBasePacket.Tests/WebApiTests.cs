using System;
using System.IO;
using XamBasePacket.Tests.Services;
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
    }
}
