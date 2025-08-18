import { Component, OnInit } from '@angular/core';
import { RestaurantItem } from '../../../models/ResturantInterface/resturant-interface';
import { ActivatedRoute, Router } from '@angular/router';
import { ListOfResturant } from '../../../services/ListOfResturant/list-of-resturant';
import { MainLayoutComponent } from '../../layout/main-layout/main-layout.component';
import { ResturanrtLogo } from '../resturanrt-logo/resturanrt-logo';
import { Footer } from '../../../components/layout/footer/footer';

@Component({
  selector: 'app-restaurant-items',
  standalone: true,
  imports: [MainLayoutComponent, ResturanrtLogo, Footer],
  templateUrl: './restaurant-items.html',
  styleUrls: ['./restaurant-items.css'], // ✅ هنا كان ناقص s
})
export class RestaurantItems implements OnInit {
  restaurantName!: string;
  items: RestaurantItem[] = [];
  loading = true;
  //cartId = 'PUT-YOUR-CART-ID-HERE'; // مؤقتًا، بعدين هنجيبه من API

  constructor(
    private route: ActivatedRoute,
    private restaurantService: ListOfResturant,
    //  private cartService: CartService,

    private router: Router // ✅ إضافة الروتر للتنقل
  ) {}

  ngOnInit(): void {
    this.restaurantName = this.route.snapshot.paramMap.get('name') || '';
    this.restaurantService
      .getItemsByRestaurantName(this.restaurantName)
      .subscribe({
        next: (res) => {
          this.items = res.$values;
          this.loading = false;
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        },
      });
  }
}
