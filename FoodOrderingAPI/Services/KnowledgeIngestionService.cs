using Newtonsoft.Json;
using FoodOrderingAPI.Models;
namespace FoodOrderingAPI.services
{
    public class KnowledgeIngestionService
    {
        private readonly ApplicationDBContext _db;
        private readonly IEmbeddingService _embedService;

        public KnowledgeIngestionService(ApplicationDBContext db, IEmbeddingService embedService)
        {
            _db = db;
            _embedService = embedService;
        }
        public async Task LoadAndStoreAsync (string fullText)
        {
            //divide text in small chunks (500 letters)
            var chunks = SplitText(fullText, 500);
            
            foreach(var chunk in chunks)
            {
                //Generate embedding to every chunk
                var embedding = await _embedService.GenerateEmbeddingAsync(chunk);

                //save chunks and json embedding in sqlserver
                _db.KnowledgeChunks.Add(new KnowledgeChunk
                {
                    Text = chunk,
                    Embedding = JsonConvert.SerializeObject(embedding)
                });

            }
            await _db.SaveChangesAsync();
        }

        public List<string> SplitText(string text, int maxLen)
        {
            var chunks = new List<string>();

            for (int i = 0; i < maxLen; i += maxLen)
            {
                chunks.Add(text.Substring(i, Math.Min(maxLen, text.Length - i)));
            }

            return chunks;
        }
    }
}
