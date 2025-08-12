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
        public bool IsValidLating(double lat, double lng)
        {
            return lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180;
        }
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
                throw new Exception("not valid latitude or longitude for destination location");
            }

            // إضافة: التحقق من الترتيب المنطقي للإحداثيات (للقاهرة/مصر)
            if (originLat > originLng || destLat > destLng)
            {
                Console.WriteLine("Warning: Coordinates might be swapped. In Egypt, latitude (~30.x) should be smaller than longitude (~31.x)");
            }

            // تحضير البيانات للإرسال
            // تغيير مهم: تأكد من ترتيب الإحداثيات - OpenRouteService يستخدم [longitude, latitude]
            var requestBody = new
            {
                coordinates = new[]
                {
                new[] { originLng, originLat }, // [lng, lat] - الطول الجغرافي أولاً
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

                // التحقق من وجود routes
                if (!root.TryGetProperty("routes", out var routes) || routes.GetArrayLength() == 0)
                {
                    throw new Exception("No routes found in API response");
                }

                // التحقق من وجود summary
                var firstRoute = routes[0];
                if (!firstRoute.TryGetProperty("summary", out var summary))
                {
                    throw new Exception("No summary found in route data");
                }

                // التحقق من وجود duration
                if (!summary.TryGetProperty("duration", out var durationProperty))
                {
                    throw new Exception("No duration found in route summary");
                }

                double durationInSeconds = durationProperty.GetDouble();
                TimeSpan durationSpan = TimeSpan.FromSeconds(durationInSeconds);

                return durationSpan;
            }
            catch (HttpRequestException ex)
            {
                // تغيير 6: معالجة أفضل للأخطاء
                Console.WriteLine($"HTTP Error: {ex.Message}");
                throw new Exception($"Failed to get travel duration: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                throw new Exception("Invalid response format from routing service", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }
        }
    }
}

   
