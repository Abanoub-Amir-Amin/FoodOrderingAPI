import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import * as signalR from '@microsoft/signalr';
import {
  DeliveryOrderDTO,
  NotificationDTO,
  OrderStatus,
  UpdateOrderStatusRequest,
} from '../../models/DeliveryManDashboardInterface/delivery-order.interface.ts';
import { AuthService } from './../auth';

@Injectable({
  providedIn: 'root',
})
export class DeliveryService {
  private readonly baseUrl = 'http://localhost:5000/api';
  private hubConnection!: signalR.HubConnection;

  // State management
  private currentOrderSubject = new BehaviorSubject<DeliveryOrderDTO | null>(
    null
  );
  private completedOrdersSubject = new BehaviorSubject<DeliveryOrderDTO[]>([]);
  private notificationsSubject = new BehaviorSubject<NotificationDTO[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  // Public observables
  public currentOrder$ = this.currentOrderSubject.asObservable();
  public completedOrders$ = this.completedOrdersSubject.asObservable();
  public notifications$ = this.notificationsSubject.asObservable();
  public loading$ = this.loadingSubject.asObservable();

  constructor(private http: HttpClient, private authService: AuthService) {
    this.initializeSignalR();
    this.loadCompletedOrdersFromStorage();
  }

  private initializeSignalR(): void {
    const token = this.authService.getAuthToken();

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/notificationHub', {
        accessTokenFactory: () => token || '',
      })
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR Connected');

        this.hubConnection.on(
          'ReceiveNotification',
          (notification: NotificationDTO) => {
            this.addNotification(notification);
            // If it's an order assignment notification, fetch the order
            if (notification.message.includes('order assigned')) {
              this.fetchCurrentOrder();
            }
          }
        );
      })
      .catch((err) => console.error('SignalR Connection Error: ', err));
  }

  // Helper method to unwrap .NET reference-wrapped responses
  private unwrapNetResponse(data: any): any {
    if (data && data.$values && Array.isArray(data.$values)) {
      return data.$values.map((item: any) => this.unwrapNetResponse(item));
    } else if (data && typeof data === 'object') {
      const unwrapped: any = {};
      for (const key in data) {
        if (key !== '$id') {
          if (data[key] && data[key].$values) {
            unwrapped[key] = this.unwrapNetResponse(data[key]);
          } else {
            unwrapped[key] = data[key];
          }
        }
      }
      return unwrapped;
    }
    return data;
  }

  // Fetch current preparing orders
  fetchCurrentOrder(): void {
    this.loadingSubject.next(true);

    this.http
      .get<any>(`${this.baseUrl}/Order/PreparingOrdersForDelivary`, {
        headers: this.authService.getAuthHeaderDeleviry(),
      })
      .pipe(
        map((response) => {
          console.log('Raw API Response:', response);

          // Unwrap the .NET reference-wrapped response
          const unwrappedData = this.unwrapNetResponse(response);
          console.log('Unwrapped Data:', unwrappedData);

          return unwrappedData as DeliveryOrderDTO[];
        })
      )
      .subscribe({
        next: (orders) => {
          console.log('Processed orders:', orders);

          // Use the updated enum value (2 for Preparing)
          const currentOrder =
            orders && orders.length > 0
              ? { ...orders[0], status: OrderStatus.Preparing }
              : null;

          console.log('Setting current order:', currentOrder);
          this.currentOrderSubject.next(currentOrder);
          this.loadingSubject.next(false);
        },
        error: (error) => {
          console.error('Error fetching current order:', error);
          this.loadingSubject.next(false);

          // Show user-friendly error message
          this.addNotification({
            userId: 'system',
            message:
              'Failed to fetch orders. Please check your connection and try again.',
          });
        },
      });
  }

  // Update order status - Fixed to use PATCH instead of PUT
  updateOrderStatus(
    orderID: string,
    status: OrderStatus,
    deliveryManId?: string
  ): Observable<any> {
    // Get deliveryManId from authService if not provided
    const finalDeliveryManId = deliveryManId || this.authService.getUserId();

    // Validate that we have a deliveryManId
    if (!finalDeliveryManId) {
      console.error('❌ DeliveryManId is required but not found');
      console.log('Debug info:', {
        providedId: deliveryManId,
        authServiceId: this.authService.getUserId(),
        localStorage: localStorage.getItem('userId'),
        sessionStorage: sessionStorage.getItem('userId'),
      });

      // Return an error observable instead of throwing
      return new Observable((observer) => {
        observer.error(
          new Error(
            'DeliveryManId is required but not found. Please log in again.'
          )
        );
      });
    }

    const request: UpdateOrderStatusRequest = {
      orderID,
      status,
      deliveryManId: finalDeliveryManId,
    };

    console.log('🚀 Updating order status with PATCH request:', request);

    // Changed from PUT to PATCH to match the backend
    return this.http.patch(
      `${this.baseUrl}/DeliveryMan/UpdateOrderStatus`,
      request,
      { headers: this.authService.getAuthHeaderDeleviry() }
    );
  }

  // Also update the progressOrder method to handle errors better
  progressOrder(deliveryManId?: string): void {
    const currentOrder = this.currentOrderSubject.value;
    if (!currentOrder) {
      console.error('❌ No current order to progress');
      this.addNotification({
        userId: 'system',
        message: 'No active order found to update.',
      });
      return;
    }

    const nextStatus = this.getNextStatus(
      currentOrder.status || OrderStatus.Preparing
    );

    console.log(
      `🔄 Progressing order ${currentOrder.orderID} to status: ${nextStatus}`
    );

    this.loadingSubject.next(true);

    this.updateOrderStatus(
      currentOrder.orderID,
      nextStatus,
      deliveryManId
    ).subscribe({
      next: (response) => {
        console.log('✅ Order status updated successfully:', response);

        if (nextStatus === OrderStatus.Delivered) {
          // Move to completed orders
          const completedOrder: DeliveryOrderDTO = {
            ...currentOrder,
            status: nextStatus,
            deliveredAt: new Date().toISOString(),
          };
          this.addCompletedOrder(completedOrder);
          this.currentOrderSubject.next(null);

          this.addNotification({
            userId: this.authService.getUserId() || 'system',
            message: `✅ Order #${currentOrder.orderNumber} delivered successfully!`,
          });
        } else {
          // Update current order status
          this.currentOrderSubject.next({
            ...currentOrder,
            status: nextStatus,
          });

          this.addNotification({
            userId: this.authService.getUserId() || 'system',
            message: `📋 Order #${
              currentOrder.orderNumber
            } is now ${this.getStatusText(nextStatus)}`,
          });
        }
        this.loadingSubject.next(false);
      },
      error: (error) => {
        console.error('❌ Error updating order status:', error);
        this.loadingSubject.next(false);

        // Handle specific error cases
        let errorMessage = 'Failed to update order status. ';

        if (error.message && error.message.includes('DeliveryManId')) {
          errorMessage = 'Authentication error: Please log in again.';
        } else if (error.status === 401) {
          errorMessage += 'Authentication failed. Please log in again.';
        } else if (error.status === 403) {
          errorMessage += 'You are not authorized to perform this action.';
        } else if (error.status === 404) {
          errorMessage += 'Order not found or already processed.';
        } else if (error.status === 500) {
          errorMessage += 'Server error. Please try again in a few moments.';
        } else {
          errorMessage += 'Please check your connection and try again.';
        }

        this.addNotification({
          userId: 'system',
          message: errorMessage,
        });
      },
    });
  }

  // Updated to use new enum values
  private getNextStatus(currentStatus: OrderStatus): OrderStatus {
    switch (currentStatus) {
      case OrderStatus.Preparing: // 2
        return OrderStatus.Out_for_Delivery; // 3
      case OrderStatus.Out_for_Delivery: // 3
        return OrderStatus.Delivered; // 4
      default:
        return currentStatus;
    }
  }

  getStatusText(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Preparing: // 2
        return 'Preparing';
      case OrderStatus.Out_for_Delivery: // 3
        return 'On Route';
      case OrderStatus.Delivered: // 4
        return 'Delivered';
      default:
        return 'Unknown';
    }
  }

  getNextButtonText(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Preparing: // 2
        return 'Start Delivery';
      case OrderStatus.Out_for_Delivery: // 3
        return 'Mark as Delivered';
      default:
        return 'Complete';
    }
  }

  // Completed orders management
  private addCompletedOrder(order: DeliveryOrderDTO): void {
    const currentCompleted = this.completedOrdersSubject.value;
    const updated = [order, ...currentCompleted];
    this.completedOrdersSubject.next(updated);
    this.saveCompletedOrdersToStorage(updated);
  }

  private saveCompletedOrdersToStorage(orders: DeliveryOrderDTO[]): void {
    try {
      localStorage.setItem('delivery_completed_orders', JSON.stringify(orders));
    } catch (error) {
      console.error('Error saving to localStorage:', error);
    }
  }

  private loadCompletedOrdersFromStorage(): void {
    try {
      const stored = localStorage.getItem('delivery_completed_orders');
      if (stored) {
        const orders = JSON.parse(stored) as DeliveryOrderDTO[];
        this.completedOrdersSubject.next(orders);
      }
    } catch (error) {
      console.error('Error loading completed orders from storage:', error);
    }
  }

  // Notifications management
  private addNotification(notification: NotificationDTO): void {
    const current = this.notificationsSubject.value;
    this.notificationsSubject.next([notification, ...current]);
  }

  clearNotifications(): void {
    this.notificationsSubject.next([]);
  }

  // Send notification (for testing)
  sendNotification(notification: NotificationDTO): Observable<any> {
    return this.http.post(`${this.baseUrl}/Notification/Notify`, notification);
  }
}
