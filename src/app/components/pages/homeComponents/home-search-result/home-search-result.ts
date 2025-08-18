import { Component, Input, OnChanges, OnDestroy } from '@angular/core';
import { HomeSearchService } from '../../../../services/home-search-service';
import { of, Subject } from 'rxjs';
import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  takeUntil,
} from 'rxjs/operators';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-home-search-result',
  imports: [RouterLink, CommonModule],
  templateUrl: './home-search-result.html',
  styleUrl: './home-search-result.css',
})
export class HomeSearchResult implements OnChanges {
  @Input() searchTerm: string = '';

  items: any[] = [];
  loading: boolean = false;
  hasError: boolean = false;

  constructor(private itemService: HomeSearchService, private router: Router) {}

  ngOnChanges() {
    if (this.searchTerm && this.searchTerm.trim() !== '') {
      this.searchItems();
    } else {
      this.items = [];
    }
  }

  searchItems() {
    this.loading = true;
    this.hasError = false;

    this.itemService.search(this.searchTerm).subscribe({
      next: (response) => {
        const categoryItems = response.byCategory?.$values || [];
        const restaurantItems = response.byRestaurant?.$values || [];

        const allItems = [...categoryItems, ...restaurantItems];
        this.items = this.removeDuplicateItems(allItems);

        this.loading = false;
      },
      error: (error) => {
        console.error('Something Went Wrong:', error);
        this.hasError = true;
        this.loading = false;
        this.items = [];
      },
    });
  }

  removeDuplicateItems(items: any[]): any[] {
    const uniqueItems: any[] = [];
    const seenIds = new Set();

    for (const item of items) {
      if (!seenIds.has(item.itemID)) {
        seenIds.add(item.itemID);
        uniqueItems.push(item);
      }
    }

    return uniqueItems;
  }

  onImageError(event: any) {
    event.target.src = 'assets/images/no-image.png'; // Default Picture
  }

  GoToCart(): void {
    this.router.navigate(['/shoppingCart']);
  }
}
