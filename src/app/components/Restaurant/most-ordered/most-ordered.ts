import { Component, Input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { HttpClient } from '@angular/common/http';

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
export class MostOrdered implements OnInit {
  @Input() restaurantId!: string;
  mostOrdered: any[] = [];

  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5000/api';

  ngOnInit() {
    this.http
      .get<any[]>(`${this.baseUrl}/restaurant/${this.restaurantId}/items/most-ordered`)
      .subscribe(data => this.mostOrdered = data);
  }
}
