export interface ChatMessage {
  id?: number;
  sender: string;
  message: string;
  timeStamp: Date;
  conversationId: string;
}

export interface ChatSession {
  conversationId: string;
  title: string;
  lastMessage: string;
  timeStamp: Date;
}
