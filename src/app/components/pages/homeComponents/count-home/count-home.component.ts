import { Component, AfterViewInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import ScrollReveal from 'scrollreveal';

@Component({
  selector: 'app-count-home',
  imports: [],
  standalone: true,
  templateUrl: './count-home.component.html',
  styleUrl: './count-home.component.css',
})
export class CountHomeComponent implements AfterViewInit {
  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  ngAfterViewInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const sr = ScrollReveal({
        origin: 'top',
        distance: '60px',
        duration: 2500,
        delay: 300,
        reset: false,
      });
      sr.reveal('.contact_data', {
        delay: 500,
        distance: '100px',
        origin: 'right',
      });
      sr.reveal('.contact_image', {
        delay: 400,
        distance: '100px',
        origin: 'left',
      });
    }
  }
}
