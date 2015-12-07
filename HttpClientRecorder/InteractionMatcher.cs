using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HttpClientRecorder.SerializedEntities;

namespace HttpClientRecorder
{
    internal class InteractionMatcher
    {
        public Interaction GetMatchingInteraction(List<Interaction> interactions, HttpRequestMessage request)
        {
            return interactions.FirstOrDefault(interaction => IsInteractionMatch(interaction, request));
        }

        private bool IsInteractionMatch(Interaction interaction, HttpRequestMessage request)
        {
            if (interaction.Request.Uri != request.RequestUri.AbsoluteUri) return false;
            if (interaction.Request.Method != request.Method.Method) return false;
            if (interaction.Request.Body != request.Content.AsString()) return false;
            return true;
        }
    }
}
