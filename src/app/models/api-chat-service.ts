export interface RAGResponse {
  answer: string;
  sources?: string[]; // المصادر اللي جاب منها المعلومات
  retrievedDocs?: any[]; // المستندات اللي استرجعها
  isRAGEnabled: boolean; // هل RAG شغال ولا لأ
}
