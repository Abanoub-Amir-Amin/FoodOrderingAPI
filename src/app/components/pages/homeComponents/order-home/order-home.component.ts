import { Component, AfterViewInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import ScrollReveal from 'scrollreveal';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-home',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './order-home.component.html',
  styleUrl: './order-home.component.css',
})
export class OrderHomeComponent implements AfterViewInit {
  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private router: Router
  ) {}

  ngAfterViewInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const sr = ScrollReveal({
        origin: 'top',
        distance: '60px',
        duration: 2500,
        delay: 300,
        reset: false,
      });

      sr.reveal('restaurant_data', { origin: 'right' });
      sr.reveal('.restaurant_info', { origin: 'left' });
      sr.reveal('.restaurant_img-1', {
        delay: 1000,
        distance: '0px',
        scale: 0,
        rotate: { z: -45 },
      });

      sr.reveal('.restaurant_img-2', {
        delay: 1200,
        distance: '0px',
        scale: 0,
        rotate: { z: 45 },
      });

      sr.reveal('.restaurant_dam-1', {
        delay: 1400,
        distance: '0px',
        scale: 0,
        rotate: { z: 45 },
      });

      sr.reveal('.restaurant_dam-3', {
        delay: 1600,
        distance: '0px',
        scale: 0,
        rotate: { z: 45 },
      });

      sr.reveal('.restaurant_dam-2', {
        delay: 1800,
        distance: '0px',
        scale: 0,
        rotate: { z: 45 },
      });

      sr.reveal('.restaurant_dam-4', {
        delay: 2000,
        distance: '0px',
        scale: 0,
        rotate: { z: 45 },
      });
    }
  }

  GoToRestaurant(): void {
    this.router.navigate(['/getAllResturant']);
  }
}
