using CSharpFunctionalExtensions;
using Degree.Services.Interfaces;
using System.Net.Http.Headers;

namespace Degree.Services
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpService> _logger;
        private HttpClient? _client;

        private const string _sendRequestResultMessage = "Sending request completed with a code:";

        public HttpService(IHttpClientFactory httpClientFactory, ILogger<HttpService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private HttpClient CreateClient() => _httpClientFactory.CreateClient();

        public async Task<Result<string>> SendGetRequestAsync(HttpRequestMessage requestMessage, CancellationToken token)
        {
            try
            {
                _client ??= CreateClient();

                _logger.LogInformation("Sending request");

                var response = await _client.SendAsync(requestMessage, token);
                _logger.LogInformation($"Sending completed with code: {response.StatusCode}");

                if (!response.IsSuccessStatusCode) return Result.Failure<string>($"{_sendRequestResultMessage} {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync(token);

                return Result.Success(content);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while sending request: {ex.Message}");
                return Result.Failure<string>(ex.Message);
            }
        }

        public Result<HttpRequestMessage> CreateGetRequest(string url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return Result.Success(request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creating request: {ex.Message}");
                return Result.Failure<HttpRequestMessage>(ex.Message);
            }
        }
    }
}
