import {
  Component,
  OnInit,
  AfterViewInit,
  ChangeDetectorRef,
  ChangeDetectionStrategy,
  inject,
} from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { CommonModule } from '@angular/common';
import { DashboardAnalytics } from './../dashboard-analytics/dashboard-analytics';
import { DashboardSummaryComponent } from './../dashboard-summary/dashboard-summary'; // UI component import here
import { DashboardSummaryService, DashboardSummaryDto } from './../../../services/dashboard-summary.service'; // service import here
import { MostOrdered } from './../most-ordered/most-ordered';
import { AuthService } from '../../../services/auth';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-restaurant-dashboard',
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
  standalone: true,
  imports: [
    RouterModule,
    MatSidenavModule,
    CommonModule,
    MatToolbarModule,
    MatProgressSpinnerModule,
    DashboardAnalytics,
    MostOrdered,
    DashboardSummaryComponent, 
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RestaurantDashboardComponent implements OnInit, AfterViewInit {
  restaurant: any = null;
  isLoading = true;
  restaurantSummary: DashboardSummaryDto | null = null; 

  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private cdr = inject(ChangeDetectorRef);
  private dashboardSummaryService = inject(DashboardSummaryService); // inject the service here

  private baseUrl = 'http://localhost:5000/api';

  ngOnInit(): void {
    const restaurantId = this.authService.getUserId();
    console.log("Restaurant ID", restaurantId);

    if (restaurantId) {
      this.loadRestaurant(restaurantId);

      this.dashboardSummaryService.getSummary(restaurantId).subscribe({
        next: (summary) => {
          console.log("summary",summary);
          this.restaurantSummary = summary;
          this.cdr.markForCheck();
        },
        error: (error) => {
          console.error('Failed to load summary', error);
          this.cdr.markForCheck();
        }
      });
    } else {
      this.isLoading = false;
    }
  }

  ngAfterViewInit(): void {
    this.cdr.detectChanges();
  }

  private getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('authToken');
    console.log("token info:", token);
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }

  private loadRestaurant(id: string): void {
    this.isLoading = true;
    const headers = this.getAuthHeaders();
    this.http.get<any>(`${this.baseUrl}/restaurant/${id}`, { headers }).subscribe({
      next: (data) => {
        console.log("Restaurant Data:", data);
        console.log('Loaded restaurant ID:', this.restaurant?.restaurantID);
        this.restaurant = data;
        this.isLoading = false;
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
