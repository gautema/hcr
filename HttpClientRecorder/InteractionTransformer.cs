using System.Net.Http;
using System.Net.Http.Headers;
using HttpClientRecorder.SerializedEntities;

namespace HttpClientRecorder
{
    public class InteractionTransformer
    {
        public Interaction GetInteraction(HttpRequestMessage request, HttpResponseMessage response)
        {
            var interaction = new Interaction
            {
                Request = new Request
                {
                    Body = request.Content.AsString(),
                    Uri = request.RequestUri.AbsoluteUri,
                    Method = request.Method.Method
                },
                Response = new Response
                {
                    Body = response.Content.AsString(),
                    StatusCode = response.StatusCode
                }
            };
            interaction.Response.Headers.Add("Content-Type", response.Content.Headers.ContentType.MediaType);
            return interaction;
        }

        public HttpResponseMessage GetResponseFromInteraction(Interaction interaction)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(interaction.Response.Body),
                StatusCode = interaction.Response.StatusCode
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(interaction.Response.Headers["Content-Type"]);
            return response;
        }


    }
}
