using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HttpClientRecorder.Tests
{
    public class HcrHttpMessageHandlerTests
    {
        private readonly HttpClient _client;

        public HcrHttpMessageHandlerTests()
        {
            //Clean up
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.json"))
            {
                File.Delete(file);
            }
            _client = new HttpClient(new HcrHttpMessageHandler());
        }

        [Fact]
        public async Task Should_get_result()
        {
            var result = await _client.GetAsync("https://github.com/gautema/CQRSlite");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var content = await result.Content.ReadAsStringAsync();
            Assert.True(content.Length > 100);
        }

        [Fact]
        public async Task Should_get_result_from_complex_url()
        {
            var result = await _client.GetAsync("https://github.com/gautema/CQRSlite/graphs/code-frequency?something=34&ha=er");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var content = await result.Content.ReadAsStringAsync();
            Assert.True(content.Length > 100);
        }

        [Fact]
        public async Task Should_save_response()
        {
            await _client.GetAsync("https://github.com/gautema/CQRSlite");
            Assert.True(File.ReadAllBytes("githubcom.json").Length > 100);
        }

        [Fact]
        public async Task Should_get_saved()
        {
            File.WriteAllText("wwwvgno.json", GetJson());
            var result = await _client.GetAsync("http://www.vg.no");
            var content = await result.Content.ReadAsStringAsync();
            Assert.Equal(6, content.Length);
        }

        private string GetJson()
        {
            return @"[
                      {
                        ""Request"": {
                          ""Headers"": { },
                          ""Body"": null,
                          ""Method"": ""GET"",
                          ""Uri"": ""http://www.vg.no/""
                        },
                        ""Response"": {
                          ""Headers"": { },
                          ""Body"": ""blabla"",
                          ""StatusCode"": 200
                        }
                      }
                    ]
                ";
        }

    }
}
