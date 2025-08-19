import { Component, inject, OnInit } from '@angular/core';
import { RestaurantItem, ResturantInterface } from '../../../models/ResturantInterface/resturant-interface';
import { ActivatedRoute } from '@angular/router';
import { ListOfResturant, Review } from '../../../services/ListOfResturant/list-of-resturant';
import { MainLayoutComponent } from "../../layout/main-layout/main-layout.component";
import { FooterComponent } from "../../layout/footer/footer.component";

import { ShoppingCartDto, ShoppingCartItemAddedDTO } from '../../../models/DTO.model';
import { ShoppingCart } from '../../../services/shoppingCart/shopping-cart';
import { ToastService } from '../../../services/toast-service';
import { AuthService } from '../../../services/auth';
import { RelativeTimePipe } from '../../pipes/pipes/relative-time-pipe';
import { Rating } from '../rating/rating';
import { ReviewService } from '../../../services/review/review-service';
@Component({
  selector: 'app-resturant-all-details',
  standalone: true,
  imports: [MainLayoutComponent, FooterComponent, RelativeTimePipe, Rating],
  templateUrl: './resturant-all-details.html',
  styleUrl: './resturant-all-details.css'
})
export class ResturantAllDetails implements OnInit {

  restaurant!: ResturantInterface;
  items: RestaurantItem[] = [];
  reviews: Review[] = [];
  loading: boolean = true;
 private auth = inject(AuthService);
  constructor(private route: ActivatedRoute, private restaurantService: ListOfResturant ,private cartservices: ShoppingCart,
      private toastservice:ToastService , private ReviewService : ReviewService) {}

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
        //this.reviews = data.$values ?? []
       this.reviews = (data.$values ?? []).filter(r => r.restaurantID === this.restaurant?.id);
      },
      error: (err) => console.error('Error fetching reviews', err)
    });
  }
    
  async AddToCart(itemId: string,preferences:string): Promise<void> {
    // debugger;
    const cart=await this.GetCart();
    if(cart?.cartID == null){
      console.log("cannot get cart")
    }
    else{
      const cartitem:ShoppingCartItemAddedDTO={
        itemID:itemId,
        cartID:cart?.cartID,
        preferences:preferences
      }
      this.cartservices.addItemToCart(cartitem).subscribe({
        next:(res)=>{
          console.log(res)
            // this.messageservice.add({severity:'success',summary:"Add To Cart",detail:"Item Added To Shopping Cart Successfully"})
          this.toastservice.showSuccess("Item Added To Shopping Cart Successfully","Add To Cart")
        },
        error:(err)=>{
          console.log(err);
        if(err.status === 401){
          
          this.toastservice.showError("You need to signin or register before you can Add Items To cart","Login needed")
      }
          else if(err.status===400)
          this.toastservice.showError(err.error,"Add To Cart failed")
         
          else
          this.toastservice.showError("failed to Add item To  shopping cart","Add To Cart failed")
  
        }
      })
    }}
    
   
  get currentCustomerId(): string {
    return this.auth.getUserId() ?? '';   // ✅ استخدمنا getUserId
  }
    GetCart():Promise<ShoppingCartDto>{
    return new Promise((resolve,reject) =>{
      this.cartservices.GetCart().subscribe({
        next:(res) =>{
          this.loading=false;
          console.log(res?.cartID);
          console.log(res?.shoppingCartItems.$values);
          resolve(res);
        },
        error:(err) => {
          console.log(err);
        if(err.status === 401){
          
          this.toastservice.showError("You need to signin or register before you can get cart","Login needed")
      }
          else if(err.status===400)
          this.toastservice.showError(err.error,"get Cart failed")
         
          else
          this.toastservice.showError("failed to get shopping cart","get Cart failed")
  
          reject(err);
        }
      });
  
    });    
   }
    deleteReview(reviewId: string): void {
  this.ReviewService.deleteReview(reviewId).subscribe({
    next: () => {
      // بعد ما يتحذف من السيرفر، نشيله من الليست المحلية
      this.reviews = this.reviews.filter(r => r.reviewID !== reviewId);
      this.toastservice.showSuccess("Review deleted successfully", "Delete Review");
    },
    error: (err) => {
      console.error('Error deleting review', err);
      this.toastservice.showError("Failed to delete review", "Delete Review");
    }
  });
}

}