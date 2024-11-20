using CSharpFunctionalExtensions;

namespace Degree.Services.Interfaces
{
    public interface IHttpService
    {
        Task<Result<string>> SendGetRequestAsync(HttpRequestMessage requestMessage, CancellationToken token);
        Result<HttpRequestMessage> CreateGetRequest(string url);
    }
}
