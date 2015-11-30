using System.Linq;

namespace HttpClientRecorder
{
    internal static class Helpers
    {
        public static string StripSpecialChars(this string str)
        {
            return new string(str.Where(char.IsLetterOrDigit).ToArray());
        }
    }
}