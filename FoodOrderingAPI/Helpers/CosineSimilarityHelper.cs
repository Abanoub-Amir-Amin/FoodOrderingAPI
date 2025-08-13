namespace FoodOrderingAPI.Helpers
{
    public static class CosineSimilarityHelper
    {
        //calculate similarity if it is equal 1 very similar 0 dissimilar
        public static float CosineSimilarity (List<float> vectorA , List<float> vectorB)
        {
            if(vectorA.Count != vectorB.Count)
            {
                throw new ArgumentException("Vectors must be the same length");
            }

            float dotProduct = 0f;
            float magA = 0f;
            float magB = 0f;

            for (int i = 0; i < vectorA.Count; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magA += vectorA[i] * vectorA[i];
                magB += vectorB[i] * vectorB[i];
            }

            return dotProduct / (float)(Math.Sqrt(magA) * Math.Sqrt(magB) + 1e-10);
        }

    }
}
