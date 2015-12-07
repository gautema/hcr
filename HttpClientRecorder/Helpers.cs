using System.Linq;
using System.Net.Http;

namespace HttpClientRecorder
{
    internal static class Helpers
    {
        public static string StripSpecialChars(this string str)
        {
            return new string(str.Where(char.IsLetterOrDigit).ToArray());
        }

        public static string AsString(this HttpContent content)
        {
            return content?.ReadAsStringAsync().Result;
        }
    }
}