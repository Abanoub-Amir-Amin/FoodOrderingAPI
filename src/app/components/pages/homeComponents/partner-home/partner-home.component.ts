import { Component, Inject, PLATFORM_ID, AfterViewInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule, NgIf, isPlatformBrowser } from '@angular/common';
import ScrollReveal from 'scrollreveal';
@Component({
  selector: 'app-partner-home',
  imports: [RouterLink],
  templateUrl: './partner-home.component.html',
  styleUrl: './partner-home.component.css',
})
export class PartnerHomeComponent implements AfterViewInit {
  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  ngAfterViewInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      // ScrollReveal configuration
      const sr = ScrollReveal({
        origin: 'top',
        distance: '60px',
        duration: 2500,
        delay: 300,
        reset: false,
      });
      sr.reveal('.partner-card--dark', {
        delay: 400,
        distance: '100px',
        origin: 'left',
      });

      sr.reveal('.partner-card--yellow', {
        delay: 400,
        distance: '100px',
        origin: 'right',
      });
    }
  }
}