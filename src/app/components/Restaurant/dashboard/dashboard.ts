import { Component, OnInit, AfterViewInit, ChangeDetectorRef, ChangeDetectionStrategy, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { CommonModule } from '@angular/common';
import { DashboardAnalytics } from './../dashboard-analytics/dashboard-analytics';
import { DashboardSummary } from './../dashboard-summary/dashboard-summary';
import { MostOrdered } from './../most-ordered/most-ordered';
import { AuthService } from '../../../services/auth';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-restaurant-dashboard',
  templateUrl: './dashboard.html',  // Use relative path, typically './dashboard.html'
  styleUrls: ['./dashboard.css'],
  standalone: true,
  imports: [
    RouterModule,
    MatSidenavModule,
    CommonModule,
    MatToolbarModule,
    MatProgressSpinnerModule,
    DashboardSummary,
    DashboardAnalytics,
    MostOrdered,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RestaurantDashboardComponent implements OnInit, AfterViewInit {
  restaurant: any = null;
  isLoading = true;

  // Inject via field injection (Angular 14+)
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private baseUrl = 'http://localhost:5000/api';

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    const restaurantId = this.authService.getUserId();
    console.log('Restaurant ID', restaurantId);

    if (restaurantId) {        
      this.loadRestaurant(restaurantId);
    } else {
      this.isLoading = false;
    }
  }

  ngAfterViewInit(): void {
    // Trigger change detection to avoid ExpressionChangedAfterItHasBeenCheckedError
    this.cdr.detectChanges();
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('Token') || localStorage.getItem('token');
    console.log('Token Info', token);
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }

  private loadRestaurant(id: string | null): void {
    if (!id) {
      this.isLoading = false;
      return;
    }

    this.isLoading = true;
    const headers = this.getAuthHeaders();

    this.http.get<any>(`${this.baseUrl}/restaurant/${id}`, { headers }).subscribe({
      next: (data) => {
        this.restaurant = data;
        this.isLoading = false;
        // Because OnPush strategy is used, mark for check to update view
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Error loading restaurant:', err);
        this.restaurant = null;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
    });
  }
}
