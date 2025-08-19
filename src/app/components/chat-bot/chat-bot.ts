import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  ElementRef,
  AfterViewChecked,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ChatService } from '../../services/chatBot/chat-service';
import { ChatMessage, ChatSession } from '../../models/chatBot/chat.model';

@Component({
  selector: 'app-chat-bot',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-bot.html',
  styleUrl: './chat-bot.css',
})
export class ChatBot implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('chatContainer') chatContainer!: ElementRef;
  @ViewChild('messageInput') messageInput!: ElementRef;

  messages: ChatMessage[] = [];
  chatSessions: ChatSession[] = [];
  currentMessage = '';
  selectedModel = 'gemini';
  isLoading = false;
  sidebarVisible = true;
  ////////////////////
  chatVisible = false; // Main chat visibility
  currentConversationId = '';

  private subscriptions: Subscription[] = [];

  constructor(private chatService: ChatService) {}

  ngOnInit() {
    // Subscribe to messages
    const messagesSubscription = this.chatService.messages$.subscribe(
      (messages) => {
        this.messages = messages;
      }
    );

    // Subscribe to loading state
    const loadingSubscription = this.chatService.isLoading$.subscribe(
      (loading) => {
        this.isLoading = loading;
      }
    );

    // Subscribe to sessions
    const sessionsSubscription = this.chatService.sessions$.subscribe(
      (sessions) => {
        this.chatSessions = sessions;
      }
    );

    this.subscriptions.push(
      messagesSubscription,
      loadingSubscription,
      sessionsSubscription
    );
    this.currentConversationId = this.chatService.getCurrentConversationId();
    ////////////////////////////////////
    // Set sidebar visible by default on desktop
    if (window.innerWidth > 768) {
      this.sidebarVisible = true;
    } else {
      this.sidebarVisible = false;
    }

    // Add welcome message
    setTimeout(() => {
      if (this.messages.length === 0) {
        this.addWelcomeMessage();
      }
    }, 500);
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  //toggle
  // Toggle main chat visibility
  toggleChat() {
    this.chatVisible = !this.chatVisible;

    // If opening chat, focus input after animation
    if (this.chatVisible) {
      setTimeout(() => {
        if (this.messageInput) {
          this.messageInput.nativeElement.focus();
        }
      }, 300);
    }
  }

  // Send message
  sendMessage() {
    if (!this.currentMessage.trim() || this.isLoading) return;

    const message = this.currentMessage.trim();
    this.currentMessage = '';
    this.adjustTextareaHeight();

    this.chatService.sendMessage(message);
  }

  // Handle enter key
  onKeyPress(event: KeyboardEvent) {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  // Auto-resize textarea
  adjustTextareaHeight(event?: Event) {
    const textarea = event
      ? (event.target as HTMLTextAreaElement)
      : this.messageInput?.nativeElement;
    if (textarea) {
        textarea.style.height = 'auto';
    if (textarea.scrollHeight > 300) {
textarea.style.height = textarea.scrollHeight + 'px';  
    } else {
    textarea.style.height = 'auto';
    }
    }
  }

  // Check if message is from user
  isUserMessage(message: ChatMessage): boolean {
    return !message.sender.startsWith('ai');
  }

  // Format message time
  formatTime(timeStamp: Date): string {
    return new Date(timeStamp).toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  // Scroll to bottom
  private scrollToBottom(): void {
    try {
      if (this.chatContainer) {
        this.chatContainer.nativeElement.scrollTop =
          this.chatContainer.nativeElement.scrollHeight;
      }
    } catch (err) {
      console.error('Error scrolling:', err);
    }
  }

  // Sidebar functions
  toggleSidebar() {
    this.sidebarVisible = !this.sidebarVisible;
  }

  // Close chat
  closeChat() {
    this.chatVisible = false;
  }

  // Close chat if clicked outside
  closeChatIfClickedOutside(event: Event) {
    // This will only trigger if the overlay is clicked
    this.closeChat();
  }
  ///////////////////////

  closeSidebar() {
    this.sidebarVisible = false;
  }

  // Session management
  selectSession(conversationId: string) {
    this.chatService.switchConversation(conversationId);
    this.currentConversationId = conversationId;
    this.closeSidebar();
  }

  createNewChat() {
    this.chatService.createNewConversation();
    this.currentConversationId = this.chatService.getCurrentConversationId();
    this.closeSidebar();
  }

  // Add welcome message
  private addWelcomeMessage() {
    const welcomeMessage: ChatMessage = {
      sender: 'ai',
      message: "Hello! I'm your AI assistant. How can I help you today?",
      timeStamp: new Date(),
      conversationId: this.currentConversationId,
    };
    this.messages = [welcomeMessage];
  }

  // Track by functions for performance
  trackByMessageId(index: number, message: ChatMessage): any {
    return message.id || index;
  }

  trackByConversationId(index: number, session: ChatSession): any {
    return session.conversationId;
  }
}
