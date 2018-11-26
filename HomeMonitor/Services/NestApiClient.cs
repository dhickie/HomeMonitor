using HomeMonitor.Models;
using HomeMonitor.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HomeMonitor.Services
{
    public class NestApiClient : INestApiClient
    {
        private const string API_BASE = "https://developer-api.nest.com/";

        private readonly HttpClient _httpClient;
        private readonly AuthenticationHeaderValue _authHeader;
        private string _redirectedUrl;

        public NestApiClient(IOptions<NestOptions> options)
        {
            // Be sure to disallow automatic redirects - we have to re-add the auth header manually
            var httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            _httpClient = new HttpClient(httpClientHandler);
            _authHeader = new AuthenticationHeaderValue("Bearer", options.Value.AccessToken);
        }

        public async Task<NestResponse> GetNestState()
        {
            var response = await SendRequest(_redirectedUrl ?? API_BASE);
            var body = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.TemporaryRedirect)
            {
                // We can reuse the redirect URI next time
                _redirectedUrl = response.Headers.Location.ToString();
                response = await SendRequest(_redirectedUrl);
                body = await response.Content.ReadAsStringAsync();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"An error occured calling the Nest API: {body}");
            }

            return JsonConvert.DeserializeObject<NestResponse>(body);
        }

        private async Task<HttpResponseMessage> SendRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = _authHeader;
            return await _httpClient.SendAsync(request);
        }
    }
}
