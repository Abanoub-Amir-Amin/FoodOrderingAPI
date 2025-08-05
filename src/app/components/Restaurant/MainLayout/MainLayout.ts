import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';

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
    MatButtonModule,
  ],
  templateUrl: './MainLayout.html',
  styleUrls: ['./MainLayout.css'],
})
export class MainLayoutComponent {
  restaurant: any;
  isDarkMode = false;

  toggleTheme() {
    this.isDarkMode = !this.isDarkMode;
    document.body.classList.toggle('dark-theme', this.isDarkMode);
  }

  logout() {
    // implement logout logic (e.g., authService.logout(); router.navigate(['/login']))
  }
}
