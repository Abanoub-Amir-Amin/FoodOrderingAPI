import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HomeSearchResult } from '../home-search-result/home-search-result';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'app-food-home',
  imports: [FormsModule, HomeSearchResult, NgIf, CommonModule],

  templateUrl: './food-home.component.html',
  styleUrl: './food-home.component.css',
})
export class FoodHomeComponent {
  clearSearch() {
    this.searchTerm = '';
  }

  onSearchInput() {
    console.log('Search term:', this.searchTerm); // عشان تتأكد
  }

  searchTerm: string = '';

  updateSearchTerm(event: any) {
    this.searchTerm = event.target.value;
    console.log('Current search term:', this.searchTerm);
  }
}
