using System.Collections.Generic;
using System.Net;

namespace HttpClientRecorder.SerializedEntities
{
    public class Response
    {
        public Response()
        {
            Headers = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}