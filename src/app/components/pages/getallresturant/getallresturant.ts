// src/app/components/GetAllResturant/getallresturant.component.ts

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ResturantInterface } from '../../../models/ResturantInterface/resturant-interface';
import { ListOfResturant } from '../../../services/ListOfResturant/list-of-resturant';
import { ResturanrtLogo } from '../resturanrt-logo/resturanrt-logo';
import { ActivatedRoute, Router } from '@angular/router';
import { NavbarComponent } from '../../layout/main-layout/navbar/navbar.component';
import { Footer } from '../../layout/footer/footer';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-getallresturant',
  standalone: true,
  imports: [CommonModule, ResturanrtLogo, NavbarComponent, Footer],
  templateUrl: './getallresturant.html',
  styleUrls: ['./getallresturant.css'],
})
export class Getallresturant implements OnInit {
  restaurants: ResturantInterface[] = [];

  constructor(
    private restaurantService: ListOfResturant,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
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
    this.router.navigate(['/restaurant', name, 'items']);
  }
}
