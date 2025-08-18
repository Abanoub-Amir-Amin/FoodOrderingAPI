import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard-summary',
  templateUrl: './dashboard-summary.html',
  styleUrls: ['./dashboard-summary.css'],
  standalone: true,
  imports: [CommonModule, MatCardModule],
})
export class DashboardSummary {
  @Input() restaurant: any;
}
