namespace FoodOrderingAPI.Services
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class OpenRouteService : IOpenRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenRouteService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _apiKey = config["OpenRouteService:ApiKey"];
        }
        public bool IsValidLating(double lat, double lng)
        {
            return lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180;
        }

        //public async Task<TimeSpan> GetTravelDurationAsync(
        //    double originLat, double originLng,
        //    double destLat, double destLng)
        //{
        //    string url = "https://api.openrouteservice.org/v2/directions/driving-car";
        //    if (!IsValidLating(originLat, originLng))
        //    {
        //        throw new Exception("not valid latitude or lngtitude for orignal location");
        //    }
        //    if (!IsValidLating(destLat, destLng))
        //    {
        //        throw new Exception("not valid latitude or lngtitude for destination location");


        //    }

        //    var requestBody = new
        //    {
        //        coordinates = new[]
        //        {
        //        new[] { originLng, originLat }, // [lng, lat]
        //        new[] { destLng, destLat }
        //    }
        //    };

        //    var jsonContent = new StringContent(
        //        JsonSerializer.Serialize(requestBody),
        //        Encoding.UTF8,
        //        "application/json"
        //    );

        //    _httpClient.DefaultRequestHeaders.Clear();
        //    _httpClient.DefaultRequestHeaders.Authorization =
        //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        //    var response = await _httpClient.PostAsync(url, jsonContent);
        //    var responseBody = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine(response.StatusCode);
        //    Console.WriteLine(responseBody);
        //    response.EnsureSuccessStatusCode();

        //    using var stream = await response.Content.ReadAsStreamAsync();
        //    using var doc = await JsonDocument.ParseAsync(stream);

        //    var root = doc.RootElement;

        //    var summary = root
        //        .GetProperty("routes")[0]
        //        .GetProperty("summary");

        //    double durationInSeconds = summary.GetProperty("duration").GetDouble();
        //    TimeSpan durationspan = TimeSpan.FromSeconds(durationInSeconds);
        //    return durationspan;

        //} 

        public async Task<TimeSpan> GetTravelDurationAsync(
     double originLat, double originLng,
     double destLat, double destLng)
        {
            // تغيير 1: إضافة الـ API key كـ parameter في الـ URL
            string url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={_apiKey}";

            // التحقق من صحة الإحداثيات
            if (!IsValidLating(originLat, originLng))
            {
                throw new Exception("not valid latitude or longitude for original location");
            }
            if (!IsValidLating(destLat, destLng))
            {
                throw new Exception("not valid latitude or lngtitude for destination location");


            }

            // تحضير البيانات للإرسال
            // تغيير مهم: تأكد من ترتيب الإحداثيات - OpenRouteService يستخدم [longitude, latitude]
            var requestBody = new
            {
                coordinates = new[]
                {
                new[] { originLng, originLat }, // [lng, lat]
                new[] { destLng, destLat }
            }
            };

            // إضافة debugging للتأكد من الترتيب
            Console.WriteLine($"Sending coordinates: Origin[{originLng}, {originLat}], Destination[{destLng}, {destLat}]");
            Console.WriteLine($"This translates to: Origin(lat:{originLat}, lng:{originLng}), Destination(lat:{destLat}, lng:{destLng})");

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            // تغيير 2: إزالة الـ Authorization header لأننا هنستخدم API key في الـ URL
            _httpClient.DefaultRequestHeaders.Clear();
            // لا نحتاج authorization header لأن الـ key موجود في الـ URL

            try
            {
                var response = await _httpClient.PostAsync(url, jsonContent);

                // تغيير 3: إضافة debugging أفضل
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status Code: {response.StatusCode}");
                Console.WriteLine($"Response: {responseBody}");

                // تغيير 4: معالجة أفضل للأخطاء المختلفة
                if (!response.IsSuccessStatusCode)
                {
                    // التحقق من نوع الخطأ بناءً على الـ response body
                    if (responseBody.Contains("Route could not be found"))
                    {
                        throw new Exception($"No route found between the specified locations. This could be due to: isolated areas, water bodies, or locations too close together. Original coordinates: ({originLat}, {originLng}) to ({destLat}, {destLng})");
                    }
                    else if (responseBody.Contains("code\":2009"))
                    {
                        throw new Exception($"Routing service cannot find a valid path between the locations. Please check if both locations are accessible by car.");
                    }
                    else
                    {
                        throw new HttpRequestException($"API request failed with status {response.StatusCode}: {responseBody}");
                    }
                }

                // تغيير 5: معالجة أكثر أماناً للـ JSON
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

            var summary = root
                .GetProperty("routes")[0]
                .GetProperty("summary");
            
            double durationInSeconds = summary.GetProperty("duration").GetDouble();
            TimeSpan durationspan = TimeSpan.FromSeconds(durationInSeconds);
            return durationspan;
            
        }
    }

   
}
