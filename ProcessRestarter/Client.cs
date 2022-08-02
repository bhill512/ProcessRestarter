﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
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
        private readonly int maxRetries = 3;

        public Client(ILogger logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _retryPolicy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(maxRetries, times =>
                TimeSpan.FromMilliseconds(times * 100));
        }

        [return: MaybeNull]
        public async Task<PlexLibraries> GetPlexLibraries(string plexUrl, string xPlexToken)
        {
            var client = _clientFactory.CreateClient();
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var uri = new Uri($"{plexUrl}/library/sections/?X-Plex-Token={xPlexToken}");
                var response = await client.GetAsync(uri);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.Information($"{plexUrl} is unresponsive after {maxRetries} tries");
                    var plexLibraries = new PlexLibraries();
                    plexLibraries.MediaContainer.Title1 = "";
                    return plexLibraries;
                }

                var responseString = await response.Content.ReadAsStringAsync();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(responseString);

                var json = JsonConvert.SerializeXmlNode(doc);

                return JsonConvert.DeserializeObject<PlexLibraries>(json);
            });
        }
    }
}
