import { Component, OnInit, inject } from '@angular/core';
import { AuthService } from '../../../services/auth';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';

import { DashboardSummary } from './../dashboard-summary/dashboard-summary';
import { DashboardAnalytics } from '../dashboard-analytics/dashboard-analytics';
import { MostOrdered } from '../most-ordered/most-ordered';
import { SnackbarService } from '../../../services/snackbar';

@Component({
  selector: 'app-restaurant-dashboard',
  standalone: true,
  imports: [
    RouterModule,
    CommonModule,
    MatSidenavModule,
    MatIconModule,
    MatToolbarModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    DashboardSummary,
    DashboardAnalytics,
    MostOrdered,
  ],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
})
export class RestaurantDashboard implements OnInit {
  user: any;
  restaurant: any;  // This should include at least { logoUrl, restaurantName, isActive, restaurantId }
  summary: any;
  mostOrderedItems: any;
  isLoading = true;
  isDarkMode = false;

  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5000/api';

  constructor(
    private auth: AuthService,
    private router: Router,
    private snackbar: SnackbarService
  ) {}

  ngOnInit(): void {
    const userId = this.auth.getUserId();
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }

    this.isLoading = true;

    /******helper function to create headers with Authorization Bearer token****/
    const headers = this.getAuthHeaders();
    this.http.get<any>(`${this.baseUrl}/restaurant/${userId}`, { headers }).subscribe({
      next: (data) => {
        this.restaurant = data;
        this.user = data.User ?? data.user;

        this.loadDashboardData(userId);
      },
      error: () => {
        this.router.navigate(['/login']);
      },
    });
  }
  /********************************************************************/

  loadDashboardData(userId: string): void {
    if (!userId) {
      this.snackbar.show('User ID is missing, cannot load dashboard details.');
      this.isLoading = false;
      return;
    }

    const headers = this.getAuthHeaders();

    let summaryLoaded = false;
    let itemsLoaded = false;

    this.http.get(`${this.baseUrl}/restaurant/${userId}/dashboard-summary`, { headers }).subscribe({
      next: (summary) => {
        this.summary = summary;
        summaryLoaded = true;
        this.checkLoadingComplete(summaryLoaded, itemsLoaded);
      },
      error: () => {
        this.snackbar.show('Failed to load dashboard summary.');
        summaryLoaded = true;
        this.checkLoadingComplete(summaryLoaded, itemsLoaded);
      },
    });

    this.http.get(`${this.baseUrl}/restaurant/${userId}/items/most-ordered`, { headers }).subscribe({
      next: (items) => {
        this.mostOrderedItems = items;
        itemsLoaded = true;
        this.checkLoadingComplete(summaryLoaded, itemsLoaded);
      },
      error: () => {
        this.snackbar.show('Failed to load most ordered items.');
        itemsLoaded = true;
        this.checkLoadingComplete(summaryLoaded, itemsLoaded);
      },
    });
  }

  private checkLoadingComplete(summaryLoaded: boolean, itemsLoaded: boolean): void {
    if (summaryLoaded && itemsLoaded) {
      this.isLoading = false;
    }
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;
    document.body.classList.toggle('dark-theme', this.isDarkMode);
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('Token') || localStorage.getItem('token');
    if (token) {
      return new HttpHeaders({
        Authorization: `Bearer ${token}`,
      });
    }
    return new HttpHeaders(); // empty if no token
  }
}
