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
            var req = GetRecordedResponse(request);
            if (req != null)
                return req;

            return await CacheResponse(request, cancellationToken);
        }

        private async Task<HttpResponseMessage> CacheResponse(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var filename = GetFileName(request);
            var interaction = GetSerializeableObject(request, response);
            List<Interaction> interactions;
            if (File.Exists(filename))
            {
                var file = File.ReadAllText(filename);
                interactions = JsonConvert.DeserializeObject<List<Interaction>>(file);
            }
            else
            {
                interactions = new List<Interaction>();
            }
            interactions.Add(interaction);
            var json = JsonConvert.SerializeObject(interactions, Formatting.Indented);
            File.WriteAllText(filename, json);
            return response;
        }

        private Interaction GetSerializeableObject(HttpRequestMessage request, HttpResponseMessage response)
        {
            return new Interaction
            {
                Request = new Request
                {
                    Body = GetContent(request.Content),
                    Uri = request.RequestUri.AbsoluteUri,
                    Method = request.Method.Method
                },
                Response = new Response
                {
                    Body = GetContent(response.Content),
                    StatusCode = response.StatusCode
                }
            };
        }

        private string GetContent(HttpContent content)
        {
            return content?.ReadAsStringAsync().Result;
        }

        private string GetFileName(HttpRequestMessage request)
        {
            return request.RequestUri.Host.StripSpecialChars() + ".json";
        }

        private HttpResponseMessage GetRecordedResponse(HttpRequestMessage request)
        {
            var filename = GetFileName(request);
            if (!File.Exists(filename)) return null;
            var file = File.ReadAllText(filename);
            var interactions = JsonConvert.DeserializeObject<List<Interaction>>(file);
            var interaction = GetMatchingInteraction(interactions, request);
            if (interaction == null) return null;
            var msg = GetResponseFromInteraction(interaction);

            return msg;
        }

        private Interaction GetMatchingInteraction(List<Interaction> interactions, HttpRequestMessage request)
        {
            return interactions.FirstOrDefault(interaction => IsInteractionMatch(interaction, request));
        }

        private bool IsInteractionMatch(Interaction interaction, HttpRequestMessage request)
        {
            if (interaction.Request.Uri != request.RequestUri.AbsoluteUri) return false;
            if (interaction.Request.Method != request.Method.Method) return false;
            if (interaction.Request.Body != GetContent(request.Content)) return false;
            return true;
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
    }
}
