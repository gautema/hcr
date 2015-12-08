using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HttpClientRecorder.Tests
{
    public class HcrHttpMessageHandlerTests
    {
        private HttpClient _client;

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
        public async Task Should_get_same_result_second_time()
        {
            var result = await _client.GetAsync("https://github.com/gautema/CQRSlite");
            var result2 = await _client.GetAsync("https://github.com/gautema/CQRSlite");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var content = await result.Content.ReadAsStringAsync();
            var content2 = await result2.Content.ReadAsStringAsync();
            Assert.Equal(content, content2);
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
        public async Task Should_save_second_interaction_to_file()
        {
            await _client.GetAsync("https://patentimages.storage.googleapis.com/pdfs/USRE45599.pdf");
            await _client.GetAsync("https://patentimages.storage.googleapis.com/pdfs/USRE45589.pdf");
            Assert.True(File.ReadAllBytes("patentimagesstoragegoogleapiscom.json").Length > 100);
        }

        [Fact]
        public async Task Should_get_consistent_file_size()
        {
            var result = await _client.GetAsync("http://pimg-fpiw.uspto.gov/fdd/34/039/091/0.pdf");
            var content = await result.Content.ReadAsStringAsync();
            Assert.Equal(1275298, content.Length);
            Assert.Equal("application/pdf", result.Content.Headers.ContentType.MediaType);
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

        [Fact]
        public async Task Should_get_contenttype()
        {
            File.WriteAllText("wwwvgno.json", GetJson());
            var result = await _client.GetAsync("http://www.vg.no");
            Assert.Equal("text/plain", result.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task Should_set_filelocation()
        {
            _client = new HttpClient(new HcrHttpMessageHandler(new HcrSettings {FileLocation = Path.GetTempPath() }));
            await _client.GetAsync("http://www.vg.no");
            Assert.True(File.Exists(Path.GetTempPath() + "wwwvgno.json"));
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
                          ""Headers"": { ""Content-Type"": ""text/plain""},
                          ""Body"": ""blabla"",
                          ""StatusCode"": 200
                        }
                      }
                    ]
                ";
        }
    }
}
