namespace FoodOrderingAPI.Services
{
    public interface IOpenRouteService
    {
        public Task<TimeSpan> GetTravelDurationAsync(double originLat, double originLng,
            double destLat, double destLng);
    }
}
