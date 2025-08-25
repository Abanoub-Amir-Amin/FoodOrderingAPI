import { Component, inject, OnInit } from '@angular/core';
import { RestaurantItem, ResturantInterface } from '../../../models/ResturantInterface/resturant-interface';
import { ActivatedRoute } from '@angular/router';
import { ListOfResturant } from '../../../services/ListOfResturant/list-of-resturant';
import { MainLayoutComponent } from "../../layout/main-layout/main-layout.component";
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemDto, ItemUpdateDto, ShoppingCartDto, ShoppingCartItemAddedDTO } from '../../../models/DTO.model';
import { ShoppingCart } from '../../../services/shoppingCart/shopping-cart';
import { ToastService } from '../../../services/toast-service';
import { AuthService } from '../../../services/auth';
import { RelativeTimePipe } from '../../pipes/pipes/relative-time-pipe';
import { Rating } from '../rating/rating';
import { ReviewService } from '../../../services/review/review-service';
import { CustomCurrencyPipe } from "../../pipes/custom-currency-pipe";
import { Footer } from "../../layout/footer/footer";
import { NavbarComponent } from "../../layout/main-layout/navbar/navbar.component";
import * as signalR from "@microsoft/signalr";

@Component({
  selector: 'app-resturant-all-details',
  standalone: true,
  imports: [MainLayoutComponent, RelativeTimePipe, Rating, CustomCurrencyPipe, Footer, NavbarComponent ,CommonModule ],
  templateUrl: './resturant-all-details.html',
  styleUrl: './resturant-all-details.css'
})
export class ResturantAllDetails implements OnInit {

  restaurant!: ResturantInterface;
  items: RestaurantItem[] = [];
  loading: boolean = true;
  reviews: any[] = [];
  connection!:signalR.HubConnection;
  private auth = inject(AuthService);

  constructor(
    private route: ActivatedRoute,
    private restaurantService: ListOfResturant,
    private cartservices: ShoppingCart,
    private toastservice: ToastService,
    private ReviewService: ReviewService
  ) {}

  async ngOnInit(): Promise<void> {
    const restaurantName = this.route.snapshot.paramMap.get('name')!;
    // جلب بيانات المطعم
    await new Promise((resolve, reject) => {this.restaurantService.getAllRestaurants().subscribe({
      next: (data) => {
        const restaurants = data.$values ?? [];
        this.restaurant = restaurants.find(r => r.restaurantName === restaurantName)!;
        resolve(data);
      },
      error: (err) => {console.error('Error fetching restaurant details', err);reject(err)}
    });
  });
    // جلب الأطباق
    await this.restaurantService.getItemsByRestaurantName(restaurantName).subscribe({
      next: (data) => {
        this.items = data.$values ?? [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching items', err);
        this.loading = false;
      }
    
    });
    // ✅ استدعاء فانكشن الريفيوهات
    this.getReviews();
  }

  // ✅ فانكشن مستقلة لجلب الريفيوهات
  getReviews() {
    this.restaurantService.getAllReviews().subscribe({
      next: (data) => {
        console.log("All Reviews",data)
        this.reviews = (data.$values ?? []).filter(r => r.restaurantID === this.restaurant?.id);
        console.log(`reviews for restaurant id =${this.restaurant.id} : ${this.reviews}`)
      },
      error: (err) => console.error('Error fetching reviews', err)
    });
  }

  async AddToCart(itemId: string, preferences: string): Promise<void> {
    const cart = await this.GetCart();
    if (cart?.cartID == null) {
      console.log("cannot get cart")
    } else {
      const cartitem: ShoppingCartItemAddedDTO = {
        itemID: itemId,
        cartID: cart?.cartID,
        preferences: preferences
      }
      this.cartservices.addItemToCart(cartitem).subscribe({
        next: (res) => {
          console.log(res)
          this.toastservice.showSuccess("Item Added To Shopping Cart Successfully", "Add To Cart")
        },
        error: (err) => {
          console.log(err);
          if (err.status === 401) {
            this.toastservice.showError("You need to signin or register before you can Add Items To cart", "Login needed")
          }
          else if (err.status === 400)
            this.toastservice.showError(err.error, "Add To Cart failed")
          else
            this.toastservice.showError("failed to Add item To  shopping cart", "Add To Cart failed")
        }
      })
    }
  }

  get currentCustomerId(): string {
    return this.auth.getUserId() ?? '';
  }

  GetCart(): Promise<ShoppingCartDto> {
    return new Promise((resolve, reject) => {
      this.cartservices.GetCart().subscribe({
        next: (res) => {
          this.loading = false;
          console.log(res?.cartID);
          console.log(res?.shoppingCartItems.$values);
          resolve(res);
        },
        error: (err) => {
          console.log(err);
          if (err.status === 401) {
            this.toastservice.showError("You need to signin or register before you can get cart", "Login needed")
          }
          else if (err.status === 400)
            this.toastservice.showError(err.error, "get Cart failed")
          else
            this.toastservice.showError("failed to get shopping cart", "get Cart failed")

          reject(err);
        }
      });
    });
  }

  deleteReview(reviewId: string) {
    
      this.ReviewService.deleteReview(reviewId).subscribe({
        next: () => {
          console.log('Review deleted successfully');
           this.toastservice.showSuccess("Review Deleted Sucsses", "Review Deleted")
          // ✅ إعادة تحميل الريفيوهات من الـ API
          this.getReviews();
        },
        error: (err) => {
          console.error('Error deleting review', err);
        }
      });
    }
  
}
