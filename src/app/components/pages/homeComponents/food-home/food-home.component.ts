import { Component, Inject, PLATFORM_ID, AfterViewInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HomeSearchResult } from '../home-search-result/home-search-result';
import { CommonModule, NgIf, isPlatformBrowser } from '@angular/common';
import ScrollReveal from 'scrollreveal';

@Component({
  selector: 'app-food-home',
  standalone: true,
  imports: [FormsModule, HomeSearchResult, NgIf, CommonModule],
  templateUrl: './food-home.component.html',
  styleUrl: './food-home.component.css',
})
export class FoodHomeComponent implements AfterViewInit {
  searchTerm: string = '';

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  clearSearch() {
    this.searchTerm = '';
  }

  onSearch(event: Event) {
    event.preventDefault();
    console.log('Search initiated with term:', this.searchTerm);

    // setTimeout(() => {
    //   this.searchTerm = '';
    // }, 900);
  }
  onSearchInput() {
    console.log('Search term:', this.searchTerm);
  }

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

      sr.reveal('.home_data');
      sr.reveal('.home_dish', {
        delay: 500,
        distance: '100px',
        origin: 'bottom',
      });
      sr.reveal('.home_burger', {
        delay: 1200,
        distance: '100px',
        duration: 1500,
      });
      sr.reveal('.home_ingredient', { delay: 1600, interval: 100 });
    }
  }
}
