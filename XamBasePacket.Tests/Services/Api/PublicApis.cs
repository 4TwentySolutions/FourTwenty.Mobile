﻿using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace XamBasePacket.Tests.Services.Api
{
    public interface IGoogleApi
    {
        [Get("/")]
        Task<string> GetRawGoogle(CancellationToken cancellationToken);
    }
    public interface IPlaceholderApi
    {
        [Get("/todos/1")]
        Task<string> GetTodos(CancellationToken cancellationToken);
        [Get("/posts")]
        Task<string> Posts(CancellationToken cancellationToken);
        [Get("/comments")]
        Task<string> Comments(CancellationToken cancellationToken);
    }
}