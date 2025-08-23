import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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
    this.fetchCompletedOrders();
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
            if (notification.message.includes('order assigned')) {
              this.fetchCurrentOrder();
            }
          }
        );
      })
      .catch((err) => console.error('SignalR Connection Error: ', err));
  }

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

  // =========================
  // 📌 Fetch current order
  // =========================
  fetchCurrentOrder(): void {
    this.loadingSubject.next(true);

    this.http
      .get<any>(`${this.baseUrl}/Order/PreparingOrdersForDelivary`, {
        headers: this.authService.getAuthHeaderDeleviry(),
      })
      .pipe(
        map((response) => {
          const unwrappedData = this.unwrapNetResponse(response);
          return unwrappedData as DeliveryOrderDTO[];
        })
      )
      .subscribe({
        next: (orders) => {
          const currentOrder =
            orders && orders.length > 0
              ? { ...orders[0], status: OrderStatus.Preparing }
              : null;

          this.currentOrderSubject.next(currentOrder);
          this.loadingSubject.next(false);
        },
        error: (error) => {
          console.error('Error fetching current order:', error);
          this.loadingSubject.next(false);
          this.addNotification({
            userId: 'system',
            message:
              'Failed to fetch orders. Please check your connection and try again.',
          });
        },
      });
  }

  // =========================
  // 📌 Fetch completed orders from API (بدل localStorage)
  // =========================
  fetchCompletedOrders(): void {
    this.http
      .get<any>(`${this.baseUrl}/Order/DelivaredOrdersForDelivary`, {
        headers: this.authService.getAuthHeaderDeleviry(),
      })
      .pipe(map((response) => this.unwrapNetResponse(response)))
      .subscribe({
        next: (orders: DeliveryOrderDTO[]) => {
          this.completedOrdersSubject.next(orders || []);
        },
        error: (error) => {
          console.error('❌ Error fetching completed orders:', error);
          this.completedOrdersSubject.next([]); // fallback empty
        },
      });
  }

  // =========================
  // 📌 Update order status
  // =========================
  updateOrderStatus(
    orderID: string,
    status: OrderStatus,
    deliveryManId?: string
  ): Observable<any> {
    const finalDeliveryManId = deliveryManId || this.authService.getUserId();

    if (!finalDeliveryManId) {
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

    return this.http.patch(
      `${this.baseUrl}/DeliveryMan/UpdateOrderStatus`,
      request,
      { headers: this.authService.getAuthHeaderDeleviry() }
    );
  }

  // =========================
  // 📌 Progress order
  // =========================
  progressOrder(deliveryManId?: string): void {
    const currentOrder = this.currentOrderSubject.value;
    if (!currentOrder) {
      this.addNotification({
        userId: 'system',
        message: 'No active order found to update.',
      });
      return;
    }

    const nextStatus = this.getNextStatus(
      currentOrder.status || OrderStatus.Preparing
    );

    this.loadingSubject.next(true);

    this.updateOrderStatus(
      currentOrder.orderID,
      nextStatus,
      deliveryManId
    ).subscribe({
      next: (response) => {
        if (nextStatus === OrderStatus.Delivered) {
          this.currentOrderSubject.next(null);
          this.addNotification({
            userId: this.authService.getUserId() || 'system',
            message: `✅ Order #${currentOrder.orderNumber} delivered successfully!`,
          });

          // ✅ بعد ما يوصل Delivered، هنعمل إعادة تحميل الـ Completed Orders من الـ API
          this.fetchCompletedOrders();
        } else {
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
        this.addNotification({
          userId: 'system',
          message: 'Failed to update order status. Please try again.',
        });
      },
    });
  }

  // =========================
  // 📌 Helpers
  // =========================
  private getNextStatus(currentStatus: OrderStatus): OrderStatus {
    switch (currentStatus) {
      case OrderStatus.Preparing:
        return OrderStatus.Out_for_Delivery;
      case OrderStatus.Out_for_Delivery:
        return OrderStatus.Delivered;
      default:
        return currentStatus;
    }
  }

  getStatusText(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Preparing:
        return 'Preparing';
      case OrderStatus.Out_for_Delivery:
        return 'On Route';
      case OrderStatus.Delivered:
        return 'Delivered';
      default:
        return 'Unknown';
    }
  }

  getNextButtonText(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Preparing:
        return 'Start Delivery';
      case OrderStatus.Out_for_Delivery:
        return 'Mark as Delivered';
      default:
        return 'Complete';
    }
  }

  // =========================
  // 📌 Notifications
  // =========================
  private addNotification(notification: NotificationDTO): void {
    const current = this.notificationsSubject.value;
    this.notificationsSubject.next([notification, ...current]);
  }

  clearNotifications(): void {
    this.notificationsSubject.next([]);
  }

  sendNotification(notification: NotificationDTO): Observable<any> {
    return this.http.post(`${this.baseUrl}/Notification/Notify`, notification);
  }
}
