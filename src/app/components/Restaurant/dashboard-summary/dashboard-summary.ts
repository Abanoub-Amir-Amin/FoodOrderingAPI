import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { DashboardSummaryDto } from '../../../models/DTO.model';

@Component({
  selector: 'app-dashboard-summary',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  templateUrl: './dashboard-summary.html',
  styleUrls: ['./dashboard-summary.css'],
})
export class DashboardSummaryComponent {
  @Input()
  restaurantSummary!: DashboardSummaryDto | null;
restaurant: any;
}
