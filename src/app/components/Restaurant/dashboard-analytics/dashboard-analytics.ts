// dashboard-analytics.ts
import { Component, Input, OnInit, OnChanges, SimpleChanges, ChangeDetectorRef, inject } from '@angular/core';
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
  imports: [
    CommonModule,
    MatCardModule,
    NgxChartsModule,
  ],
})
export class DashboardAnalytics implements OnInit, OnChanges {
  @Input() restaurantId!: string;
  weeklyOrders: any[] = [];

  private authService = inject(AuthService);
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5000/api';
  private cdRef = inject(ChangeDetectorRef);

  ngOnInit() {
    if (this.restaurantId) {
      this.loadOrders();
    } else {
      this.weeklyOrders = [];
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['restaurantId'] && !changes['restaurantId'].isFirstChange()) {
      if (this.restaurantId) {
        console.log("Restaurant ID", this.restaurantId);
        this.loadOrders();
      } else {
        this.weeklyOrders = [];
        this.cdRef.detectChanges();
      }
    }
  }

  private loadOrders(): void {
    if (!this.restaurantId) {
      console.log("Restaurant ID",this.restaurantId);
      this.weeklyOrders = [];
      this.cdRef.detectChanges();
      return;
    }

    const headers = this.getAuthHeaders();

    this.http.get<any[]>(`${this.baseUrl}/order/${this.restaurantId}/orders?status=`, { headers }).subscribe({
      next: orders => {
        if (!Array.isArray(orders)) {
          this.weeklyOrders = [];
          this.cdRef.detectChanges();
          return;
        }

        const dayMap = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
        const counts: Record<string, number> = { Mon: 0, Tue: 0, Wed: 0, Thu: 0, Fri: 0, Sat: 0, Sun: 0 };

        orders.forEach(o => {
          if (o.createdAt) {
            const date = new Date(o.createdAt);
            const dow = (date.getDay() + 6) % 7; // Monday=0 ... Sunday=6
            counts[dayMap[dow]]++;
          }
        });

        this.weeklyOrders = dayMap.map(day => ({ name: day, value: counts[day] }));

        // Trigger Angular change detection safely after update
        setTimeout(() => this.cdRef.detectChanges());
      },
      error: () => {
        this.weeklyOrders = [];
        setTimeout(() => this.cdRef.detectChanges());
      }
    });
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('Token') || localStorage.getItem('token');
    console.log('Token Info:', token);
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }
}
