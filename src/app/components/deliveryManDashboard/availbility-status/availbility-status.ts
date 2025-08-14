import { Component } from '@angular/core';
import { AvailabilityService } from '../../../services/DeliveryManDashboardService/availability-service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-availbility-status',
  imports: [FormsModule],
  templateUrl: './availbility-status.html',
  styleUrl: './availbility-status.css',
})
export class AvailbilityStatus {
  constructor(private availabilityService: AvailabilityService) {}

  isSwitched: boolean = false;

  ngOnInit(): void {
    this.showAvailabilityStatus();
  }

  showAvailabilityStatus(): void {
    this.availabilityService.getAvailabilityStatus().subscribe({
      next: (response) => {
        console.log('Availability status:', response);
        this.isSwitched = response;
      },
      error: (err) => {
        console.error('Error fetching availability status:', err);
      },
    });
  }

  switchAvailabilityStatus(isChecked: boolean): void {
    this.availabilityService.switchAvailabilityStatus(isChecked).subscribe({
      next: () => {
        console.log(isChecked);
        this.availabilityService.setAvailability(isChecked);
      },
      error: (err) => {
        console.error('Error fetching availability status:', err);
      },
    });
  }

  updateAvailabilityStatusBehaviorSubject(): void {
    this.availabilityService.setAvailability(this.isSwitched);
  }
}
