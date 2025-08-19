import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NavigationEnd } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import {MenuManagement} from '../menu-management/menu-management';
import {RestaurantDashboardComponent} from '../dashboard/dashboard';
import {DashboardAnalytics} from '../dashboard-analytics/dashboard-analytics';
import {DashboardSummaryComponent} from '../dashboard-summary/dashboard-summary';
import {MostOrdered} from '../most-ordered/most-ordered';
import {OrdersManagement} from '../orders-management/orders-management';
import {RestaurantProfileComponent} from '../restaurant-profile/restaurant-profile';

import { AuthService } from '../../../services/auth';

const navbarData = [
  {
    RouterLink: '/dashboard',
  },
  {
    RouterLink: '/orders-management',
  },
  {
    RouterLink: '/menu-management',
  },
  {
    RouterLink: '/restaurant-profile',
  },
  
];

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    MatIconModule,
    MatToolbarModule,
    MatProgressSpinnerModule,
    MatButtonModule
  ],
  templateUrl: './MainLayout.html',
  styleUrls: ['./MainLayout.css'],
})
export class MainLayoutComponent implements OnInit {
  restaurant: any | null = null;  // Initialize explicitly
  restaurantId: string | null = null;  // Use string|null type for IDs
  navData = navbarData;
  isDarkMode = false;

  private authService = inject(AuthService);
  //private router = inject(Router);

  constructor(private router: Router) {
  this.router.events.subscribe(event => {
    if (event instanceof NavigationEnd) {
      console.log('Navigated to:', event.urlAfterRedirects);
    }
  });
}
  ngOnInit() {
    this.restaurantId = this.authService.getUserId();
    console.log("this.restaurantId:",this.restaurantId);

    if (this.restaurantId) {
      this.authService.loadRestaurant(this.restaurantId).subscribe({
        next: (data) => {
          this.restaurant = data;
          console.log(this.restaurant);
        },
        error: (err) => {
          console.error('Failed to load restaurant in main layout:', err);
          this.restaurant = null;
        },
      });
    } else {
      this.restaurant = null;
    }
  }

  // toggleTheme() {
  //   this.isDarkMode = !this.isDarkMode;
  //   document.body.classList.toggle('dark-theme', this.isDarkMode);
  // }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
  
  public getImageUrl(imageFile?: string): string {
  if (!imageFile) {
    return 'public/assets/restaurantLogo.jpg';
  }
  const url = this.authService.getImageUrl(imageFile);
  return url;
}

}
