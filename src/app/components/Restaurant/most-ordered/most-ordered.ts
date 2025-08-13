// most-ordered.ts
import { Component, Input, OnInit, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-most-ordered',
  templateUrl: 'most-ordered.html',
  styleUrls: ['most-ordered.css'],
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
  ],
})
export class MostOrdered implements OnInit, OnChanges {
  @Input() restaurantId!: string;
  
  mostOrdered: any[] = [];

  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private baseUrl = 'http://localhost:5000/api';

  ngOnInit() {
    if (this.restaurantId) {
      this.loadMostOrdered();
    } else {
      this.mostOrdered = [];
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['restaurantId'] && !changes['restaurantId'].isFirstChange()) {
      if (this.restaurantId) {
        this.loadMostOrdered();
      } else {
        this.mostOrdered = [];
      }
    }
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('Token') || localStorage.getItem('token');
    console.log('Token Info', token);
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }

  private loadMostOrdered(): void {
    if (!this.restaurantId) {
      this.mostOrdered = [];
      return;
    }

    const headers = this.getAuthHeaders();
    console.log('Restaurant ID:', this.restaurantId);

    this.http.get<any[]>(`${this.baseUrl}/item/${this.restaurantId}/items/most-ordered`, { headers }).subscribe({
      next: data => {
        if (Array.isArray(data)) {
          this.mostOrdered = data;
        } else {
          this.mostOrdered = [];
        }
      },
      error: () => {
        this.mostOrdered = [];
      },
    });
  }
}
