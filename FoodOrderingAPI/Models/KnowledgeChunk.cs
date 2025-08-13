namespace FoodOrderingAPI.Models
{
    public class KnowledgeChunk
    {
        public int Id { get; set; }
        
        public string Text { get; set; } // Our data from DB to convert it to embedding chunks
        //json string 
        public string Embedding { get; set; } // save data here after be Embedding to help LLM compares it with user's question embeddings 
    }
}
