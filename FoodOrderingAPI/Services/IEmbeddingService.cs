namespace FoodOrderingAPI.services
{
    public interface IEmbeddingService
    {
        Task<List<float>> GenerateEmbeddingAsync(string text);
        Task<string> GenerateAnswerAsync(string prompt);

    }
}
