using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HttpClientRecorder.SerializedEntities;
using Newtonsoft.Json;

namespace HttpClientRecorder
{
    public class HcrHttpMessageHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!HasCachedRequest(request))
            {
                await CacheResponse(request, cancellationToken);
            }
            return GetRecordedResponse(request);
        }

        private async Task CacheResponse(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var filename = GetFileName(request);
            var serializableObject = await GetSerializeableObject(request, response);
            var json = JsonConvert.SerializeObject(serializableObject, Formatting.Indented);
            File.WriteAllText(filename, json);
        }

        private async Task<List<Interaction>> GetSerializeableObject(HttpRequestMessage request, HttpResponseMessage response)
        {
            return new List<Interaction>
            {
                new Interaction
                {
                    Request = new Request
                    {
                        Body = request.Content == null ? null : await request.Content.ReadAsStringAsync(),
                        Uri = request.RequestUri.AbsoluteUri
                    },
                    Response = new Response
                    {
                        Body = response.Content == null ? null : await response.Content.ReadAsStringAsync(),
                        StatusCode = response.StatusCode
                    }
                }
            };
        }

        private string GetFileName(HttpRequestMessage request)
        {
            return request.RequestUri.Host.StripSpecialChars() + ".json";
        }

        private HttpResponseMessage GetRecordedResponse(HttpRequestMessage request)
        {
            var filename = GetFileName(request);
            var file = File.ReadAllText(filename);
            var interactions = JsonConvert.DeserializeObject<List<Interaction>>(file);
            var msg = GetResponseFromInteraction(interactions.First());

            return msg;
        }

        private HttpResponseMessage GetResponseFromInteraction(Interaction interaction)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(interaction.Response.Body),
                StatusCode = interaction.Response.StatusCode
            };
            foreach (var header in interaction.Response.Headers)
            {
                response.Headers.Add(header.Key, header.Value);
            }
            return response;
        }

        private bool HasCachedRequest(HttpRequestMessage request)
        {
            return File.Exists(GetFileName(request));
        }
    }
}
