import { Component, Input, OnInit, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { NgxChartsModule } from '@swimlane/ngx-charts';

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
export class DashboardAnalytics implements OnInit {
  @Input() restaurantId!: string;
  weeklyOrders: any[] = [];

  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5000/api';

  ngOnInit() {
    if (!this.restaurantId) return;

    const headers = this.getAuthHeaders();

    this.http
      .get<any[]>(`${this.baseUrl}/restaurant/${this.restaurantId}/orders?status=`, { headers })
      .subscribe(orders => {
        const dayMap = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
        const counts: Record<string, number> = { Mon: 0, Tue: 0, Wed: 0, Thu: 0, Fri: 0, Sat: 0, Sun: 0 };

        orders.forEach(o => {
          if (o.createdAt) {
            const date = new Date(o.createdAt);
            const dow = (date.getDay() + 6) % 7;
            counts[dayMap[dow]]++;
          }
        });

        this.weeklyOrders = dayMap.map(d => ({ name: d, value: counts[d] }));
      });
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('Token') || localStorage.getItem('token');
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }
}
