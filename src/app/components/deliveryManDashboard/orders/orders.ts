import { Component, OnInit, OnDestroy } from '@angular/core';
import {
  DeliveryOrderDTO,
  OrderStatus,
} from '../../../models/DeliveryManDashboardInterface/delivery-order.interface.ts';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { NgClass } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { DeliveryService } from '../../../services/DeliveryManDashboardService/delivery.service';
import { trigger, style, transition, animate } from '@angular/animations';
import { AuthService } from '../../../services/auth.js';

@Component({
  selector: 'app-orders',
  imports: [CommonModule, HttpClientModule],
  providers: [DeliveryService], // ⚠️ ممكن تشيلها وتخلي السيرفس Singleton من root
  templateUrl: './orders.html',
  styleUrl: './orders.css',
  animations: [
    trigger('slideDown', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-10px)' }),
        animate(
          '300ms ease-out',
          style({ opacity: 1, transform: 'translateY(0)' })
        ),
      ]),
      transition(':leave', [
        animate(
          '300ms ease-in',
          style({ opacity: 0, transform: 'translateY(-10px)' })
        ),
      ]),
    ]),
  ],
})
export class Orders implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  activeTab = 'current';
  currentOrder: DeliveryOrderDTO | null = null;
  completedOrders: DeliveryOrderDTO[] = [];
  loading = false;
  showOrderDetails = false;
  showNotifications = false;
  notifications: any[] = [];

  private deliveryManId: string = '';

  constructor(
    private deliveryService: DeliveryService,
    private authService: AuthService
  ) {
    this.deliveryManId = this.authService.getUserId() || '';
  }

  ngOnInit(): void {
    this.subscribeToObservables();

    this.deliveryService.fetchCurrentOrder();
    this.deliveryService.fetchCompletedOrders();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private subscribeToObservables(): void {
    this.deliveryService.currentOrder$
      .pipe(takeUntil(this.destroy$))
      .subscribe((order) => (this.currentOrder = order));

    this.deliveryService.completedOrders$
      .pipe(takeUntil(this.destroy$))
      .subscribe((orders) => (this.completedOrders = orders));

    this.deliveryService.loading$
      .pipe(takeUntil(this.destroy$))
      .subscribe((loading) => (this.loading = loading));

    this.deliveryService.notifications$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notifications) => (this.notifications = notifications));
  }

  getImage(path: string | null | undefined): string {
    return this.authService.getImageUrl(path);
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
    this.showOrderDetails = false;

    if (tab === 'completed') {
      this.deliveryService.fetchCompletedOrders();
    }
  }

  toggleOrderDetails(): void {
    this.showOrderDetails = !this.showOrderDetails;
  }

  progressOrder(): void {
    this.deliveryService.progressOrder(this.deliveryManId);
  }

  getStatusText(status: OrderStatus): string {
    return this.deliveryService.getStatusText(status);
  }

  getNextButtonText(status: OrderStatus): string {
    return this.deliveryService.getNextButtonText(status);
  }

  toggleNotifications(): void {
    this.showNotifications = !this.showNotifications;
  }

  clearNotifications(): void {
    this.deliveryService.clearNotifications();
  }

  refreshCurrentOrder(): void {
    this.deliveryService.fetchCurrentOrder();
  }

  refreshCompletedOrders(): void {
    this.deliveryService.fetchCompletedOrders();
  }

  // Test notification
  testNotification(): void {
    this.deliveryService
      .sendNotification({
        userId: this.deliveryManId,
        message: 'New order assigned to you!',
      })
      .subscribe();
  }
}
