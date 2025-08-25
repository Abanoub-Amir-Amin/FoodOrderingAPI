import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, ElementRef, HostListener, Inject, Input, PLATFORM_ID, signal, ViewChild } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from '../../../services/auth';
import { Router } from '@angular/router';
import { F } from '@angular/cdk/keycodes';

// Define proper interfaces for type safety
interface Notification {
  id: string;
  message: string;
  type: 'info' | 'warning' | 'success';
  time: string;
  read: boolean;
}

@Component({
  selector: 'app-customer-header',
  imports: [CommonModule],
  templateUrl: './customer-header.html',
  styleUrl: './customer-header.css'
})
export class CustomerHeader {
//menu
isActive:boolean=false;
mobileOpen:boolean=false;
@ViewChild('menu', { static: false }) MenuEl!: ElementRef;
@ViewChild('toggle', { static: false }) ToggleEl!: ElementRef;

  connection!: signalR.HubConnection;
  @Input() collapsed = false;
  @Input() screenWidth = 0;
  isModalOpen = false;
 
  // Properly typed notifications array
  notifications: Notification[] = [];
  isBrowser: boolean;
  username:string=''
  constructor(private authService: AuthService, private router: Router ,@Inject(PLATFORM_ID) private platformId: Object) {
       this.isBrowser = isPlatformBrowser(this.platformId);

  }
 
  async ngOnInit() {
       await fetch(`http://prestoordering.somee.com/api/notification/GetNotifications/${this.authService.getUserId()}`, {
      method: "GET",
      credentials: "include"
    })
    .then(response => response.json())
    .then(data => {
      const messages = data.$values;
      console.log(messages);
      for (let i = 0; i < messages.length; i++) {
        const notification: Notification = {
          id: messages[i].notificationId,
          message: messages[i].message,
          type: 'success', // Default type, can be customized
          time: new Date(messages[i].createdAt).toLocaleTimeString(),
          read: messages[i].isRead || false
        };
        this.notifications.push(notification);
        console.log('Processed notification:', notification);
      }
    })
    .catch(err => console.error('Error fetching notifications:', err));
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://prestoordering.somee.com/notificationhub', {
        accessTokenFactory: () => this.authService.getAuthToken() ?? ''
      })
      .build();
    this.connection.start()
      .then(() => {
        console.log('SignalR connected.');
        this.connection.on('ReceiveNotification', (message: any, messageId: string) => {
          console.log('Raw message received:', message, typeof message);
          
          // Convert string message to proper notification object
          const notification: Notification = this.createNotificationObject(message);
          this.notifications.push(notification);
          console.log('Processed notification:', notification);
        });
      })
      .catch(err => console.error('SignalR connection failed:', err));
      await fetch(`http://prestoordering.somee.com/api/Customer/ByID/${this.authService.getUserId()}`, {
      method: "GET",
      headers :this.getAuthHeader()
    })
    .then(response => response.json())
    .then(data => {
      this.username=data.userName
    })
    .catch(err => console.error('Error load user data:', err));

  }

  // Helper method to convert string/any message to proper Notification object
  private createNotificationObject(message: any, messageId?: string): Notification {
    // If message is already an object with proper structure
    if (typeof message === 'object' && message.title && message.message) {
      return {
        id: messageId ?? '',
        message: message.message,
        type: message.type || 'success',
        time: message.time || this.getCurrentTime(),
        read: message.read || false
      };
    }
    
    // If message is a string, create a structured notification
    if (typeof message === 'string') {
      return {
        id: messageId ?? '',
        message: message,
        type: this.getNotificationType(message),
        time: this.getCurrentTime(),
        read: false
      };
    }

    // Fallback for other types
    return {
      id: messageId ?? '',
      message: String(message),
      type: 'success',
      time: this.getCurrentTime(),
      read: false
    };
  }

  private getCurrentTime(): string {
    return new Date().toLocaleTimeString();
  }

  private getNotificationTitle(message: string): string {
    // Derive title from message content
    if (message.toLowerCase().includes('order')) {
      return 'Order Update';
    } else if (message.toLowerCase().includes('payment')) {
      return 'Payment Notification';
    } else if (message.toLowerCase().includes('delivery')) {
      return 'Delivery Update';
    }
    return 'New Notification';
  }

  private getNotificationType(message: string): 'info' | 'warning' | 'success' {
    // Determine type based on message content
    if (message.toLowerCase().includes('success') || message.toLowerCase().includes('completed')) {
      return 'success';
    } else if (message.toLowerCase().includes('warning') || message.toLowerCase().includes('failed')) {
      return 'warning';
    }
    return 'info';
  }
 
  getHeadClass(): string {
    let styleClass = '';
    if (this.collapsed && this.screenWidth > 768) {
      styleClass = 'head-trimmed';
    } else {
      styleClass = 'head-md-screen';
    }
    return styleClass;
  }
 
  openModal() {
    console.log('Opening modal...'); // Debug log
    this.isModalOpen = true;
    document.body.style.overflow = 'hidden';
  }
 
  closeModal() {
    console.log('Closing modal...'); // Debug log
    this.isModalOpen = false;
    document.body.style.overflow = 'auto';
  }
 
  onBackdropClick(event: Event) {
    if (event.target === event.currentTarget) {
      this.closeModal();
    }
  }
 
  hasNotifications(): boolean {
    return this.notifications && this.notifications.length > 0;
  }
 
  markAllAsRead() {
    // Now this will work because notifications are proper objects
    this.notifications.forEach(async notification => {
      notification.read = true;
      await fetch(`http://prestoordering.somee.com/api/notification/MarkAsRead/${notification.id}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${this.authService.getAuthToken()}`
        }
      });
    });
    
    // Option 1: Keep notifications but mark them as read
    console.log('All notifications marked as read');
    
    // Option 2: Clear all notifications (uncomment if you want this behavior)
    // this.notifications = [];
    // console.log('All notifications cleared');
  }

  getNotificationColor(type: string): string {
    switch (type) {
      case 'info':
        return '#17a2b8';
      case 'warning':
        return '#ffc107';
      case 'success':
        return '#28a745';
      default:
        return '#6c757d';
    }
  }

  // Optional: Method to remove individual notifications
  removeNotification(notificationId: string) {
    this.notifications = this.notifications.filter(n => n.id !== notificationId);
  }

  // Optional: Get unread count for badge display
  getUnreadCount(): number {
    return this.notifications.filter(n => !n.read).length;
  }
  showOnlyUnread = false;

toggleUnreadFilter() {
  this.showOnlyUnread = !this.showOnlyUnread;
}

getFilteredNotifications(): Notification[] {
  if (this.showOnlyUnread) {
    return this.notifications.filter(n => !n.read);
  }
  return this.notifications;
}

toggleNotificationRead(notification: Notification) {
  notification.read = !notification.read;
  fetch(`http://prestoordering.somee.com/api/notification/MarkAsRead/${notification.id}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.authService.getAuthToken()}`
    }
  });
  console.log(`Notification ${notification.id} marked as ${notification.read ? 'read' : 'unread'}`);
}

clearAllNotifications() {
  if (confirm('Are you sure you want to clear all notifications?')) {
    this.notifications = [];
  }
}



//navbar
 navigate(path:string) {
    // Implement login logic or redirect to login page
    this.router.navigate([path]);
    this.closemenu()
  }

  toggleMenu(){
    // debugger;
    this.isActive=!this.isActive
    this.mobileOpen=!this.mobileOpen
    console.log(this.isActive)
    console.log(this.mobileOpen)
  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: Event) {
    const clickedInside = this.MenuEl.nativeElement.contains(event.target);
    const clickedToggle = this.ToggleEl.nativeElement.contains(event.target);
    
    if (!clickedInside && !clickedToggle&& this.mobileOpen) {
      this.mobileOpen = false;
      this.isActive=false;
    }
  }
  closemenu(){
    if(window.innerWidth<=600)
    {
      this.isActive=false;
      this.mobileOpen=false;
    }
  }
  @HostListener('window:resize')
  onResize() {
    if (window.innerWidth > 600 && this.mobileOpen) {this.mobileOpen=false;this.isActive=false}
  }
   getAuthHeader(): Headers {
    if (!this.isBrowser) return new Headers();

    const token = sessionStorage.getItem('authToken');
    // console.log('Token Info', token);
    return token
      ? new Headers({ Authorization: `Bearer ${token}` })
      : new Headers();
  }

}