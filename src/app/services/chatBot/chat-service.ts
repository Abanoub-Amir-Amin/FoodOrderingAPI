import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ApiChatService } from './api-chat-service';
import { ChatMessage, ChatSession } from '../../models/chatBot/chat.model';

// ✅ إضافة interface للـ session data
interface SessionResponse {
  sessionIds?: string[];
  sessions?: string[];
  [key: string]: any; // للسماح بـ properties إضافية
}

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private currentUser: string;
  private currentConversationId = this.generateConversationId();

  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  public messages$ = this.messagesSubject.asObservable();

  private sessionsSubject = new BehaviorSubject<ChatSession[]>([]);
  public sessions$ = this.sessionsSubject.asObservable();

  private isLoadingSubject = new BehaviorSubject<boolean>(false);
  public isLoading$ = this.isLoadingSubject.asObservable();

  constructor(private apiService: ApiChatService) {
    this.currentUser = this.getOrCreateUserId();
    this.loadChatHistory();
    this.loadSessions();
  }

  private getOrCreateUserId(): string {
    if (typeof window !== 'undefined' && sessionStorage) {
      let uid = sessionStorage.getItem('chatbotUserId');
      if (!uid) {
        uid = 'user_' + Math.random().toString(36).substring(2, 10);
        sessionStorage.setItem('chatbotUserId', uid);
      }
      return uid;
    }
    return 'server_user_' + Math.random().toString(36).substring(2, 10);
  }

  private generateConversationId(): string {
    return (
      Date.now().toString() + '_' + Math.random().toString(36).substr(2, 9)
    );
  }

  loadChatHistory() {
    this.apiService
      .getMessages(this.currentUser, this.currentConversationId)
      .subscribe({
        next: (messages) => {
          // ✅ تأكد إن الـ messages array
          const validMessages = Array.isArray(messages) ? messages : [];
          this.messagesSubject.next(validMessages);
        },
        error: (error) => {
          console.error('Error loading chat history:', error);
          this.messagesSubject.next([]); // ✅ لو فشل، حط array فاضي
        },
      });
  }

  loadSessions() {
    this.apiService.getSessions(this.currentUser).subscribe({
      next: (sessionData: SessionResponse | string[] | any) => {
        // ✅ تحديد الأنواع المحتملة
        console.log('Sessions data received:', sessionData);

        // ✅ التعامل مع أنواع مختلفة من البيانات
        let sessionIds: string[] = [];

        if (Array.isArray(sessionData)) {
          // إذا كان array مباشرة
          sessionIds = sessionData;
        } else if (sessionData && typeof sessionData === 'object') {
          // إذا كان object، جيب الـ values أو keys
          if (
            'sessionIds' in sessionData &&
            Array.isArray(sessionData.sessionIds)
          ) {
            sessionIds = sessionData.sessionIds;
          } else if (
            'sessions' in sessionData &&
            Array.isArray(sessionData.sessions)
          ) {
            sessionIds = sessionData.sessions;
          } else {
            // حول الـ object لـ array
            sessionIds = Object.keys(sessionData);
          }
        } else {
          // في حالة البيانات مش صحيحة
          console.warn('Invalid session data format:', sessionData);
          sessionIds = [];
        }

        // ✅ تأكد إن كل عنصر string
        const validSessionIds = sessionIds.filter(
          (id) => typeof id === 'string' && id.trim() !== ''
        );

        // ✅ حول لـ ChatSession objects
        const sessions: ChatSession[] = validSessionIds.map((id, index) => ({
          conversationId: id,
          title: `Chat ${index + 1}`,
          lastMessage: 'Loading...',
          timeStamp: new Date(),
        }));

        this.sessionsSubject.next(sessions);
      },
      error: (error) => {
        console.error('Error loading sessions:', error);
        this.sessionsSubject.next([]); // ✅ لو فشل، حط array فاضي
      },
    });
  }

  async sendMessage(messageText: string) {
    if (!messageText.trim()) return;

    this.isLoadingSubject.next(true);

    const userMessage: ChatMessage = {
      sender: this.currentUser,
      message: messageText,
      timeStamp: new Date(),
      conversationId: this.currentConversationId,
    };

    this.addMessageToUI(userMessage);

    try {
      await this.apiService.saveMessage(userMessage).toPromise();
      const aiResponse = await this.apiService
        .getAIResponse(messageText)
        .toPromise();

      const aiMessage: ChatMessage = {
        sender: 'ai',
        message:
          aiResponse?.answer || 'Sorry, I could not generate a response.',
        timeStamp: new Date(),
        conversationId: this.currentConversationId,
      };

      this.addMessageToUI(aiMessage);
      await this.apiService.saveMessage(aiMessage).toPromise();

      // ✅ حدث الـ sessions بعد إرسال رسالة جديدة
      this.loadSessions();
    } catch (error) {
      console.error('Error sending message:', error);
      this.addMessageToUI({
        sender: 'ai',
        message: 'Sorry, there was an error processing your request.',
        timeStamp: new Date(),
        conversationId: this.currentConversationId,
      });
    } finally {
      this.isLoadingSubject.next(false);
    }
  }

  private addMessageToUI(message: ChatMessage) {
    const currentMessages = this.messagesSubject.value;
    this.messagesSubject.next([...currentMessages, message]);
  }

  switchConversation(conversationId: string) {
    this.currentConversationId = conversationId;
    this.loadChatHistory();
  }

  createNewConversation() {
    this.currentConversationId = this.generateConversationId();
    this.messagesSubject.next([]);
    // ✅ حدث الـ sessions بعد إنشاء محادثة جديدة
    this.loadSessions();
  }

  getCurrentConversationId(): string {
    return this.currentConversationId;
  }

  getCurrentUserId(): string {
    return this.currentUser;
  }

  // ✅ دالة مساعدة للـ debugging
  debugSessionData() {
    this.apiService.getSessions(this.currentUser).subscribe({
      next: (data) => console.log('Raw session data:', data),
      error: (error) => console.error('Session debug error:', error),
    });
  }
}
