using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;
using XamBasePacket.Services.Api;

namespace XamBasePacket.Tests.Services
{

    [TestClass]
    public class ApiManagerTests : BaseApiTest
    {

        [TestMethod]
        public async Task GoogleCall()
        {
            var manager = GetApiManager();
            var result = await manager.GetRawGoogle();
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public async Task PlaceholderTodos()
        {

            var manager = GetApiManager();
            var result = await manager.GetPlaceholderTodos();
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
        [TestMethod]
        public async Task PlaceholderCommentsExtended()
        {
            var manager = GetApiManager();
            var result = await manager.GetPlaceholderComments("https://jsonplaceholder.typicode.com/");
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            var resultHttp = await manager.GetPlaceholderComments("http://jsonplaceholder.typicode.com/");
            Assert.IsNotNull(resultHttp);
            Assert.AreEqual(true, resultHttp.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, resultHttp.StatusCode);
            Assert.AreEqual(resultHttp.Content, result.Content);

            var wrongHttp = await manager.GetPlaceholderComments("http://jsonplaceholderWRONG.typicode.com/");
            Assert.IsNotNull(wrongHttp);
            Assert.AreEqual(false, wrongHttp.IsSuccess);
        }

        [TestMethod]
        public async Task PlaceholderCommentsExtendedTwice()
        {
            var manager = GetApiManager();
            var result = await manager.GetPlaceholderComments("https://jsonplaceholder.typicode.com/");
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            var result1 = await manager.GetPlaceholderComments("https://jsonplaceholder.typicode.com/");
            Assert.IsNotNull(result1);
            Assert.AreEqual(true, result1.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result1.StatusCode);
            Assert.AreEqual(result.Content, result1.Content);

            var resultHttp = await manager.GetPlaceholderComments("http://jsonplaceholder.typicode.com/");
            Assert.IsNotNull(resultHttp);
            Assert.AreEqual(true, resultHttp.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, resultHttp.StatusCode);
            Assert.AreEqual(resultHttp.Content, result.Content);

            var wrongHttp = await manager.GetPlaceholderComments("http://jsonplaceholderWRONG.typicode.com/");
            Assert.IsNotNull(wrongHttp);
            Assert.AreEqual(false, wrongHttp.IsSuccess);
        }

        [TestMethod]
        public async Task AsyncPolicyTest()
        {
            var manager = GetApiManager();
            var result = await manager.Failure();
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }


        [TestMethod]
        public async Task DisposingHandlersTest()
        {
            await MakeDisposableRequest();
        }


        [TestMethod]
        public async Task DisposingReusageHandlersTest()
        {

            await MakeDisposableRequest();

            await MakeDisposableRequest();
        }

        [TestMethod]
        public async Task DisposingAuthTest()
        {

            using (var provider = new HttpClientProvider())
            {
                var manager = GetApiManager(provider);
                var result = await manager.GetPlaceholderCommentsAuth("https://jsonplaceholder.typicode.com/");
                Assert.IsNotNull(result);
                Assert.AreEqual(true, result.IsSuccess);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            }
            using var provider2 = new HttpClientProvider();
            var manager2 = GetApiManager(provider2);
            var result2 = await manager2.GetPlaceholderCommentsAuth("https://jsonplaceholder.typicode.com/");
            Assert.IsNotNull(result2);
            Assert.AreEqual(true, result2.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result2.StatusCode);

        }
        private async Task MakeDisposableRequest()
        {
            using var provider = new HttpClientProvider();
            var manager = GetApiManager(provider);
            var result = await manager.GetPlaceholderComments("https://jsonplaceholder.typicode.com/");
            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            var resultHttp = await manager.GetPlaceholderComments("http://jsonplaceholder.typicode.com/");
            Assert.IsNotNull(resultHttp);
            Assert.AreEqual(true, resultHttp.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, resultHttp.StatusCode);
            Assert.AreEqual(resultHttp.Content, result.Content);
        }

    }
}
