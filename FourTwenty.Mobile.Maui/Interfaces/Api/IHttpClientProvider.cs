using Fusillade;

namespace FourTwenty.Mobile.Maui.Interfaces.Api
{
    public interface IHttpClientOptions : IEquatable<IHttpClientOptions>
    {
        string? Name { get; set; }
        Uri? BaseAddress { get; set; }
        HttpMessageHandler? MessageHandler { get; set; }
        Priority Priority { get; set; }
    }

    public interface IHttpClientProvider : IHttpClientFactory
    {
        HttpClient GetClient(IHttpClientOptions? options = null);
        Task<HttpClient> GetClientAsync(IHttpClientOptions? options = null);
    }
}
