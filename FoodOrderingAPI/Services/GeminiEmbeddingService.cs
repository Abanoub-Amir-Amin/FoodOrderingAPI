
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace FoodOrderingAPI.services
{
    public class GeminiEmbeddingService : IEmbeddingService
    {
        private readonly IConfiguration  _config;
        private readonly HttpClient _httpt;

        public GeminiEmbeddingService(HttpClient http , IConfiguration config)
        {
            _httpt = http;
            _config = config;
        }

        public async Task<List<float>> GenerateEmbeddingAsync(string text)
        {
            var apiKey = _config["Gemini:ApiKey"];
            var endPoint = "https://generativelanguage.googleapis.com/v1beta/models/embedding-001:embedContent";

            var payLoad = new
            {
                model = "models/embedding-001",
                content = new
                {
                    parts = new[]
                    {
                        new { text = text },
                    }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, endPoint);

            request.Headers.Add("X-goog-api-key", apiKey);
            request.Content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(payLoad),
                Encoding.UTF8,
                "application/json");

            var response = await _httpt.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if(!response.IsSuccessStatusCode)
            {
                throw new Exception($"Gemini API Error: {response.StatusCode}\n{body}");
            }

            var parsed = JObject.Parse(body);
            var values = parsed["embedding"]?["values"]?.Select(v => (float)v).ToList();

            if(values == null || values.Count == 0)
            {
                throw new Exception("Embedding extraction failed");
            }

            return values;
        }

        public async Task<string> GenerateAnswerAsync(string prompt)
        {
            var apiKey = _config["Gemini:ApiKey"];
            var endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

            var payload = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("X-goog-api-key", apiKey);
            request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _httpt.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Gemini answer error: {response.StatusCode} - {body}");

            var json = JObject.Parse(body);
            var answer = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            return answer ?? "No answer generated.";
        }


    }
}
