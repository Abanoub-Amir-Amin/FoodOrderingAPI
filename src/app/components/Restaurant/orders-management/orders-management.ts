import { Component, OnInit, inject } from '@angular/core';
import { AuthService } from '../../../services/auth';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-orders-management',
  templateUrl: './orders-management.html',
  styleUrls: ['./orders-management.css'],
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatSelectModule,
    FormsModule,
    MatProgressSpinnerModule,
  ],
})
export class OrdersManagement implements OnInit {
  restaurantId = '';
  orders: any[] = [];
  filteredOrders: any[] = [];
  selectedStatusFilter = 'All';
  allowedStatuses = ['All', 'Preparing', 'Out for Delivery', 'Delivered', 'Cancelled'];
  isLoading = false;
  error = '';

  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private snackBar = inject(MatSnackBar);
  private baseUrl = 'http://localhost:5000/api';

  ngOnInit(): void {
    this.restaurantId = this.auth.getUserId() || '';
    console.log('Restaurant ID:', this.restaurantId);
    if (!this.restaurantId) {
      this.error = 'No restaurant ID found. Please log in again.';
      return;
    }

    this.loadOrders();
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('Token') || localStorage.getItem('token');
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }

  loadOrders(): void {
    if (!this.restaurantId) {
      this.error = 'Restaurant ID missing.';
      return;
    }

    this.isLoading = true;
    this.error = '';

    let url = '';
    if (this.selectedStatusFilter === 'All') {
      url = `${this.baseUrl}/order/${this.restaurantId}/orders`;
    } else {
      url = `${this.baseUrl}/order/${this.restaurantId}/orders/status?status=${encodeURIComponent(this.selectedStatusFilter)}`;
    }

    this.http.get<any[]>(url, { headers: this.getAuthHeaders() }).subscribe({
      next: orders => {
        this.orders = Array.isArray(orders) ? orders : [];
        this.applyFilter();
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load orders';
        this.isLoading = false;
      },
    });
  }

  applyFilter(): void {
    if (this.selectedStatusFilter === 'All') {
      this.filteredOrders = this.orders;
    } else {
      this.filteredOrders = this.orders.filter(
        order => order.Status?.toLowerCase() === this.selectedStatusFilter.toLowerCase()
      );
    }
  }

  onStatusFilterChange(status: string): void {
    this.selectedStatusFilter = status;
    this.loadOrders();
  }

  acceptOrder(order: any): void {
    this.changeOrderStatus(order, 'Preparing');
  }

  finishOrder(order: any): void {
    this.changeOrderStatus(order, 'Out for Delivery');
  }

  rejectOrder(order: any): void {
    if (confirm('Are you sure you want to reject this order?')) {
      this.changeOrderStatus(order, 'Cancelled');
    }
  }

  private changeOrderStatus(order: any, newStatus: string): void {
    if (!this.restaurantId || !order.OrderID) {
      this.snackBar.open('Invalid order or restaurant ID');
      return;
    }

    const url = `${this.baseUrl}/order/${this.restaurantId}/orders/${order.OrderID}/status`;
    const body = { orderID: order.OrderID, status: newStatus };

    this.http.put(url, body, { headers: this.getAuthHeaders() }).subscribe({
      next: () => {
        this.snackBar.open(`Order #${order.OrderID} status updated to ${newStatus}`, 'Close', { duration: 4000 });
        this.loadOrders();
      },
      error: (err) => {
        const errMsg = err?.error?.error || err?.error || err.message || 'Unknown error';
        this.snackBar.open(`Failed to update order status: ${errMsg}`, 'Close', { duration: 5000 });
      },
    });
  }
}
