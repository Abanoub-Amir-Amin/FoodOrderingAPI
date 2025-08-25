import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from '../../../services/auth'; // adjust path
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-customer-inbobox',
  imports: [CommonModule],
  templateUrl: './customer-inbobox.html',
  styleUrls: ['./customer-inbobox.css']
})
export class CustomerInbobox implements OnInit, OnDestroy {

  @ViewChild('chatHistory', { static: true }) chatHistory!: ElementRef<HTMLDivElement>;
  @ViewChild('chatInput', { static: true }) chatInput!: ElementRef<HTMLTextAreaElement>;

  connection!: signalR.HubConnection;
  senderId: string | null = null;
  receiverId: string | null = null;
  messages: { content: string, isOutgoing: boolean, time: string }[] = [];

  constructor(private authService: AuthService) {}

  async ngOnInit(): Promise<void> {
    this.senderId = this.authService.getUserId();

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://prestoordering.somee.com/chathub', {
        accessTokenFactory: () => this.authService.getAuthToken() ?? ''
      })
      .build();

    const response = await fetch(`http://prestoordering.somee.com/api/chat/GetChatMessages/${this.senderId}`, {
                  method: "GET",
                  credentials: "include"
              });

    const data = await response.json();
    const messages = data.$values;

    console.log(messages);
    for (let i = 0; i < messages.length; i++) {
        if(messages[i].sender === this.senderId){
            this.addMessage(messages[i].message, true);
        } else {
            this.addMessage(messages[i].message, false);
        }
    }

    this.connection.start()
      .then(() => {
        console.log('SignalR connected.');
        this.connection.on('ReceiveMessage', (senderId: string, message: string) => {
          this.addMessage(message, false);
        });
      })
      .catch(err => console.error('SignalR connection failed:', err));
  }

  ngOnDestroy(): void {
    if (this.connection) {
      this.connection.stop();
    }
  }

  private formatTime(): string {
    const now = new Date();
    return now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  private addMessage(content: string, isOutgoing: boolean): void {
    this.messages.push({
      content,
      isOutgoing,
      time: this.formatTime()
    });

    setTimeout(() => {
      this.chatHistory.nativeElement.scrollTop = this.chatHistory.nativeElement.scrollHeight;
    }, 0);
  }

  async sendMessage(): Promise<void> {
    const input = this.chatInput.nativeElement;
    const message = input.value.trim();
    if (!message) return;

    this.addMessage(message, true);

    this.receiverId = 'Admin0';
    await fetch('http://prestoordering.somee.com/api/chat/sendmessage', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.authService.getAuthToken()}`
      },
      body: JSON.stringify({
        senderId: this.senderId,
        receiverId: this.receiverId,
        customerId: this.senderId, 
        message: message
      })
    });

    input.value = '';
    this.adjustTextareaHeight();
  }

  adjustTextareaHeight(): void {
    const input = this.chatInput.nativeElement;
    input.style.height = 'auto';
    if (input.scrollHeight > 300) {
input.style.height = input.scrollHeight + 'px';  
    } else {
    input.style.height = 'auto';
    }
    
  }

  onEnterPress(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.sendMessage();
    }
  }
}
