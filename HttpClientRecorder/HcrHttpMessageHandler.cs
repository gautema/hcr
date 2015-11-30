using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
            File.WriteAllBytes(filename, await response.Content.ReadAsByteArrayAsync());
        }

        private string GetFileName(HttpRequestMessage request)
        {
            return request.Method.Method + request.RequestUri.AbsoluteUri.StripSpecialChars() + ".hcr";
        }

        private HttpResponseMessage GetRecordedResponse(HttpRequestMessage request)
        {
            var filename = GetFileName(request);
            var file = File.ReadAllBytes(filename);
            var msg = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(file)
            };
            return msg;
        }

        private bool HasCachedRequest(HttpRequestMessage request)
        {
            return File.Exists(GetFileName(request));
        }
    }
}
