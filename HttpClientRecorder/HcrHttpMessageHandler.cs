using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientRecorder
{
    public class HcrHttpMessageHandler : HttpClientHandler
    {
        private readonly InteractionTransformer _interactionTransformer;
        private readonly InteractionMatcher _matcher;
        private readonly FileHandler _fileHandler;

        public HcrHttpMessageHandler() : this(new HcrSettings())
        {
        }

        public HcrHttpMessageHandler(HcrSettings settings)
        {
            _interactionTransformer = new InteractionTransformer();
            _matcher = new InteractionMatcher();
            _fileHandler = new FileHandler(settings.FileLocation);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return GetRecordedResponse(request) ?? await CacheResponse(request, cancellationToken);
        }

        private async Task<HttpResponseMessage> CacheResponse(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var interaction = _interactionTransformer.GetInteraction(request, response);
            _fileHandler.SaveInteraction(request.RequestUri.Host, interaction);

            return response;
        }

        private HttpResponseMessage GetRecordedResponse(HttpRequestMessage request)
        {
            var interactions = _fileHandler.GetInteractions(request.RequestUri.Host);
            if (interactions == null) return null;
            var interaction = _matcher.GetMatchingInteraction(interactions, request);
            if (interaction == null) return null;
            var msg = _interactionTransformer.GetResponseFromInteraction(interaction);

            return msg;
        }
    }
}
