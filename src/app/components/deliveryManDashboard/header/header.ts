import { NgClass } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { AvailabilityService } from '../../../services/DeliveryManDashboardService/availability-service';

@Component({
  selector: 'app-header',
  imports: [NgClass],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header implements OnInit {
  @Input() collapsed = false;
  @Input() screenWidth = 0;

  constructor(private availabilityService: AvailabilityService) {}
  isAvailable: boolean = false;

  ngOnInit(): void {
    this.showAvailabilityStatus();
    this.availabilityService.availability$.subscribe(
      (status) => (this.isAvailable = status)
    );
  }

  showAvailabilityStatus(): void {
    this.availabilityService.getAvailabilityStatus().subscribe({
      next: (response) => {
        console.log('Availability status:', response);
        this.isAvailable = response;
      },
      error: (err) => {
        console.error('Error fetching availability status:', err);
      },
    });
  }

  getHeadClass(): string {
    let styleClass = '';
    if (this.collapsed && this.screenWidth > 768) {
      styleClass = 'head-trimmed';
    } else {
      styleClass = 'head-md-screen';
    }
    return styleClass;
  }
}
