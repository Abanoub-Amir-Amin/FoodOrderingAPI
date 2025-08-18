import {
  Component,
  Input,
  OnInit,
  OnChanges,
  SimpleChanges,
  ChangeDetectorRef,
  inject,
} from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-dashboard-analytics',
  templateUrl: './dashboard-analytics.html',
  styleUrls: ['./dashboard-analytics.css'],
  standalone: true,
  imports: [CommonModule, MatCardModule, NgxChartsModule],
})
export class DashboardAnalytics implements OnInit, OnChanges {
  @Input() restaurantID!: string;
  weeklyOrders: { name: string; value: number }[] = [];

  private authService = inject(AuthService);
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5000/api';
  private cdRef = inject(ChangeDetectorRef);

  ngOnInit() {
    if (this.restaurantID) {
      this.loadOrders();
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['restaurantID'] && !changes['restaurantID'].isFirstChange()) {
      if (this.restaurantID) {
        this.loadOrders();
      } else {
        this.weeklyOrders = [];
        this.cdRef.markForCheck();
      }
    }
  }

  private loadOrders(): void {
    if (!this.restaurantID) {
      this.weeklyOrders = [];
      this.cdRef.markForCheck();
      return;
    }
    console.log("Restaurant analytics ID", this.restaurantID);
    const headers = this.getAuthHeaders();

    this.http
      .get<any>(`${this.baseUrl}/order/${this.restaurantID}/orders?status=`, { headers })
      .subscribe({
        next: (orders) => {
          console.log("orders:", orders);
          // Check for nested $values array
          const ordersArray = orders?.$values ?? orders;

          if (!Array.isArray(ordersArray)) {
            this.weeklyOrders = [];
            this.cdRef.markForCheck();
            return;
          }

          const dayMap = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
          const counts: Record<string, number> = {
            Mon: 0, Tue: 0, Wed: 0, Thu: 0, Fri: 0, Sat: 0, Sun: 0,
          };

          ordersArray.forEach(o => {
            if (o.orderDate) {
              const date = new Date(o.orderDate);
              const dow = (date.getDay() + 6) % 7; // Monday=0 ... Sunday=6
              counts[dayMap[dow]]++;
            }
          });

          this.weeklyOrders = dayMap.map(day => ({ name: day, value: counts[day] }));
          this.cdRef.markForCheck();
        },
        error: (err) => {
          console.error('Failed to load orders', err);
          this.weeklyOrders = [];
          this.cdRef.markForCheck();
        }
      });
  }

  private getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('authToken');
    console.log("token info:", token);
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }
}
