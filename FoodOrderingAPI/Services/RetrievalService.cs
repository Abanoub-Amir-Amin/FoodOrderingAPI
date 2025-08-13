using FoodOrderingAPI.Helpers;
using Newtonsoft.Json;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.services
{
    public class RetrievalService
    {
        private readonly ApplicationDBContext _db;
        private readonly IEmbeddingService _embeddingService;

        public RetrievalService(ApplicationDBContext db, IEmbeddingService embeddingService)
        {
            _db = db;
            _embeddingService = embeddingService;
        }

        public async Task<List<KnowledgeChunk>> GetTopMatchesAsync(string userQuestion, int topK = 3)
        {
            var questionEmbedding = await _embeddingService.GenerateEmbeddingAsync(userQuestion);

            var matches = new List<(KnowledgeChunk Chunk, float Similarity)>();

            foreach (var chunk in _db.KnowledgeChunks)
            {
                var vector = JsonConvert.DeserializeObject<List<float>>(chunk.Embedding);
                var similarity = CosineSimilarityHelper.CosineSimilarity(questionEmbedding, vector);
                matches.Add((chunk, similarity));
            }

            return matches
                .OrderByDescending(m => m.Similarity)
                .Take(topK)
                .Select(m => m.Chunk)
                .ToList();

        }
    }
}
