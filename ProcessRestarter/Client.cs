using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ProcessRestarter
{
    public class Client
    {
        ILogger _logger;
        IHttpClientFactory _clientFactory;
        AsyncRetryPolicy _retryPolicy;
        AsyncTimeoutPolicy _timeoutPolicy;
        private readonly int maxRetries = 3;

        public Client(ILogger logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _retryPolicy = Policy.Handle<HttpRequestException>().Or<TimeoutRejectedException>().WaitAndRetryAsync(maxRetries, times =>
                TimeSpan.FromMilliseconds(times * 100));
            _timeoutPolicy = Policy.TimeoutAsync(1);
        }

        public async Task<PlexLibraries> GetPlexLibraries(string plexUrl, string xPlexToken)
        {
            _logger.Information($"Making an API call to {plexUrl}");
            var client = _clientFactory.CreateClient();
            var uri = new Uri($"{plexUrl}/library/sections/?X-Plex-Token={xPlexToken}");

            var response = await _retryPolicy.ExecuteAsync(async () =>
                await _timeoutPolicy.ExecuteAsync(async () =>
                    await client.GetAsync(uri))
            );
            try
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(responseString);

                var json = JsonConvert.SerializeXmlNode(doc);

                var values = JsonConvert.DeserializeObject<PlexLibraries>(json);
                return values;
            }
            catch (TaskCanceledException)
            {
                _logger.Error($"Request timed out");
                return null;
            }
            catch (Exception e)
            {
                _logger.Error($"Request is wonky: {e.Message}");
                return null;
            }

        }
    }
}
