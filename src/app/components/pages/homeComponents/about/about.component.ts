import {
  Component,
  AfterViewInit,
  Inject,
  PLATFORM_ID,
  signal,
} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { NgFor } from '@angular/common';

import ScrollReveal from 'scrollreveal';

@Component({
  selector: 'app-about',
  imports: [NgFor, CommonModule],
  standalone: true,
  templateUrl: './about.component.html',
  styleUrl: './about.component.css',
})
export class AboutComponent implements AfterViewInit {
  features = [
    {
      tag: 'Zero Fees',
      title: 'No hidden charges',
      desc: 'Upfront pricing with transparent taxes and delivery costs.',
    },
    {
      tag: 'Live Tracking',
      title: 'Track every step',
      desc: 'From kitchen to your door, follow your order in real time.',
    },
    {
      tag: 'Freshness',
      title: 'Hot and tasty',
      desc: 'Partner kitchens seal freshness so your meals arrive perfect.',
    },
    {
      tag: 'Support 24/7',
      title: 'We are here',
      desc: 'Message support anytime for fast, friendly help.',
    },
  ];
  tab = signal<'why' | 'benefits' | 'support' | 'faq'>('why');

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
      sr.reveal('.card-m', { delay: 400, interval: 100 });
    }
  }
}
