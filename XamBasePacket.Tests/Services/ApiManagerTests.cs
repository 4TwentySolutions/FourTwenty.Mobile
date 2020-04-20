using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

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


    }
}
