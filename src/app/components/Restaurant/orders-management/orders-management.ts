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

  allowedStatuses = ['All', 'WaitingToConfirm', 'Preparing', 'Out_for_Delivery', 'Cancelled'];

  isLoading = false;
  error = '';

  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private snackBar = inject(MatSnackBar);
  private authService = inject(AuthService);

  private baseUrl = 'http://localhost:5000/api';

  private statusMap = new Map<any, string>([
    [1, 'WaitingToConfirm'],
    [2, 'Preparing'],
    [3, 'Out_for_Delivery'],
    [4, 'Delivered'],
    [5, 'Cancelled'],
    ['WaitingToConfirm', 'WaitingToConfirm'],
    ['Preparing', 'Preparing'],
    ['Out_for_Delivery', 'Out_for_Delivery'],
    ['Delivered', 'Delivered'],
    ['Cancelled', 'Cancelled'],
  ]);

  ngOnInit(): void {
    this.restaurantId = this.auth.getUserId() || '';
    if (!this.restaurantId) {
      this.error = 'No restaurant ID found. Please log in again.';
      return;
    }
    this.loadOrders();
  }

  private getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('authToken');
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }

  loadOrders(): void {
    console.log("this.restaurantId",this.restaurantId);
    console.log("encodeURIComponent(this.selectedStatusFilter)",this.selectedStatusFilter);
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
    this.http.get<any>(url, { headers: this.getAuthHeaders() }).subscribe({
      next: (response) => {
        this.orders = response && response.$values ? response.$values : Array.isArray(response) ? response : [];
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        this.error = `Failed to load orders: ${err.message || 'Unknown error'}`;
        this.isLoading = false;
      },
    });
  }

  getStatusString(statusCodeOrString: number | string): string {
    return this.statusMap.get(statusCodeOrString) || 'Unknown';
  }

  applyFilter(): void {
    if (this.selectedStatusFilter === 'All') {
      this.filteredOrders = this.orders;
    } else {
      this.filteredOrders = this.orders.filter(
        (order) => this.getStatusString(order.status) === this.selectedStatusFilter
      );
    }
  }

  onStatusFilterChange(status: string): void {
    this.selectedStatusFilter = status;
    this.loadOrders();
  }

  acceptOrder(order: any): void {
    this.changeOrderStatus(order, 2);
  }

  // finishOrder(order: any): void {
  //   this.changeOrderStatus(order, 3);
  // }

  rejectOrder(order: any): void {
    if (confirm('Are you sure you want to reject this order?')) {
      this.changeOrderStatus(order, 5);
    }
  }

  private extractErrorMessage(err: any): string {
    if (err?.error?.error) return err.error.error;
    if (typeof err.error === 'string') return err.error;
    if (err.message) return err.message;
    return 'Unknown error';
  }

  private changeOrderStatus(order: any, newStatusCode: number): void {
    if (!this.restaurantId || !order.orderID) {
      this.snackBar.open('Invalid order or restaurant ID', 'Close', { duration: 4000 });
      return;
    }
    console.log("this.restaurantId:",this.restaurantId);
    const url = `${this.baseUrl}/order/${this.restaurantId}/orders/${order.orderID}/status`;
    const body = { status: newStatusCode };
    console.log("status", newStatusCode);
    this.http.put(url, body, { headers: this.getAuthHeaders() }).subscribe({
      next: () => {
        this.snackBar.open(
          `Order #${order.orderNumber || order.orderID} status updated to ${this.getStatusString(newStatusCode)}`,
          'Close',
          { duration: 4000 }
        );
        this.loadOrders();
      },
      error: (err) => {
        console.log(err)
        if (err.status === 403) {
          this.snackBar.open('There is no Delivery Man available.', 'Close', { duration: 5000 });
        } else {
          const errMsg = this.extractErrorMessage(err);
          this.snackBar.open(`Failed to update order status: ${errMsg}`, 'Close', { duration: 5000 });
        }
      },
    });
  }

  public getImageUrl(imageFile?: string): string {
    return this.authService.getImageUrl(imageFile);
  }
}
