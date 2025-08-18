import { NgFor } from '@angular/common';
import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-about',
  imports: [NgFor],
  templateUrl: './about.component.html',
  styleUrl: './about.component.css',
})
export class AboutComponent {
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
}
