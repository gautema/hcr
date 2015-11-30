using System.Collections.Generic;

namespace HttpClientRecorder.SerializedEntities
{
    public class Request
    {
        public Request()
        {
            Headers = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
        public string Method { get; set; }
        public string Uri { get; set; }
    }
}