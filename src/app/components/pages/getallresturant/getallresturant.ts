// src/app/components/GetAllResturant/getallresturant.component.ts

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ResturantInterface } from '../../../models/ResturantInterface/resturant-interface';
import { ListOfResturant } from '../../../services/ListOfResturant/list-of-resturant';
import { ResturanrtLogo } from '../resturanrt-logo/resturanrt-logo';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-getallresturant',
  standalone: true,
  imports: [CommonModule, ResturanrtLogo],
  templateUrl: './getallresturant.html',
  styleUrls: ['./getallresturant.css'],
})
export class Getallresturant implements OnInit {
  restaurants: ResturantInterface[] = [];

  constructor(
    private restaurantService: ListOfResturant,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.restaurantService.getAllRestaurants().subscribe({
      next: (data) => {
        console.log('API data:', data);
        this.restaurants = data?.$values ?? [];
      },
      error: (err) => console.error('Error fetching restaurants', err),
    });
  }
  goToRestaurantItems(name: string) {
    this.router.navigate(['/restaurant/:name/items', name, 'items']);
  }
}
