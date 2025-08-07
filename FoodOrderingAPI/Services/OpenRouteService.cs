namespace FoodOrderingAPI.Services
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class OpenRouteService:IOpenRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenRouteService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _apiKey = config["OpenRouteService:ApiKey"];
        }

        public async Task<TimeSpan> GetTravelDurationAsync(
            double originLat, double originLng,
            double destLat, double destLng)
        {
            string url = "https://api.openrouteservice.org/v2/directions/driving-car";

            var requestBody = new
            {
                coordinates = new[]
                {
                new[] { originLng, originLat }, // [lng, lat]
                new[] { destLng, destLat }
            }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", _apiKey);

            var response = await _httpClient.PostAsync(url, jsonContent);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var root = doc.RootElement;

            var summary = root
                .GetProperty("features")[0]
                .GetProperty("properties")
                .GetProperty("summary");

            double durationInSeconds = summary.GetProperty("duration").GetDouble();
            TimeSpan durationspan = TimeSpan.FromSeconds(durationInSeconds);
            return durationspan;
            
        }
    }

   
}
