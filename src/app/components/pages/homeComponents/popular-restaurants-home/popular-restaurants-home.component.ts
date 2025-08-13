import { Component } from '@angular/core';
import { CommonModule } from '@angular/common'; // Needed for @for, ngIf, etc.

@Component({
  selector: 'app-popular-restaurants-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './popular-restaurants-home.component.html',
  styleUrls: ['./popular-restaurants-home.component.css']
})
export class PopularRestaurantsHomeComponent {
  
}
