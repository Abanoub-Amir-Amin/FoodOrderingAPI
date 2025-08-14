import { Component, OnInit } from '@angular/core';
import { RestaurantItem, ResturantInterface } from '../../../models/ResturantInterface/resturant-interface';
import { ActivatedRoute } from '@angular/router';
import { ListOfResturant, Review } from '../../../services/ListOfResturant/list-of-resturant';
import { MainLayoutComponent } from "../../layout/main-layout/main-layout.component";
import { FooterComponent } from "../../layout/footer/footer.component";

@Component({
  selector: 'app-resturant-all-details',
  standalone: true,
  imports: [MainLayoutComponent, FooterComponent],
  templateUrl: './resturant-all-details.html',
  styleUrl: './resturant-all-details.css'
})
export class ResturantAllDetails implements OnInit {

  restaurant!: ResturantInterface;
  items: RestaurantItem[] = [];
  reviews: Review[] = [];
  loading: boolean = true;

  constructor(private route: ActivatedRoute, private restaurantService: ListOfResturant) {}

  ngOnInit(): void {
    const restaurantName = this.route.snapshot.paramMap.get('name')!;

    // جلب بيانات المطعم
    this.restaurantService.getAllRestaurants().subscribe({
      next: (data) => {
        const restaurants = data.$values ?? [];
        this.restaurant = restaurants.find(r => r.restaurantName === restaurantName)!;
      },
      error: (err) => console.error('Error fetching restaurant details', err)
    });

    // جلب الأطباق
    this.restaurantService.getItemsByRestaurantName(restaurantName).subscribe({
      next: (data) => {
        this.items = data.$values ?? [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching items', err);
        this.loading = false;
      }
    });

    // جلب الريفيوهات
    this.restaurantService.getAllReviews().subscribe({
      next: (data) => {
        this.reviews = (data.$values ?? []).filter(r => r.restaurantID === this.restaurant?.id);
      },
      error: (err) => console.error('Error fetching reviews', err)
    });
  }
}
