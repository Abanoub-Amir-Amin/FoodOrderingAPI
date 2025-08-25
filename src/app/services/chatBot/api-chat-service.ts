import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChatMessage, ChatSession } from '../../models/chatBot/chat.model';

@Injectable({
  providedIn: 'root',
})
export class ApiChatService {
  private baseUrl = 'http://prestoordering.somee.com/api'; // Your .NET API URL

  constructor(private http: HttpClient) {}

  // Save message to backend
  saveMessage(message: ChatMessage): Observable<ChatMessage> {
    return this.http.put<ChatMessage>(`${this.baseUrl}/chatbot`, message);
  }

  // Get chat history
  getMessages(
    sender: string,
    conversationId: string
  ): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(
      `${this.baseUrl}/chatbot/sender?sender=${sender}&conversationId=${conversationId}`
    );
  }

  // Get conversation sessions
  getSessions(sender: string): Observable<string[]> {
    return this.http.get<string[]>(
      `${this.baseUrl}/chatbot/sessions?sender=${sender}`
    );
  }

  // Get AI response
  getAIResponse(question: string): Observable<{ answer: string }> {
    return this.http.get<{ answer: string }>(
      `${this.baseUrl}/retrieval/ask?q=${encodeURIComponent(question)}`
    );
  }
}
