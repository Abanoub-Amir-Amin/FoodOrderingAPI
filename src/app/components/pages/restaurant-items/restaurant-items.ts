import { Component, OnInit } from '@angular/core';
import { RestaurantItem } from '../../../models/ResturantInterface/resturant-interface';
import { ActivatedRoute, Router } from '@angular/router';
import { ListOfResturant } from '../../../services/ListOfResturant/list-of-resturant';
import { ShoppingCart } from '../../../services/shoppingCart/shopping-cart';
import { ShoppingCartDto, ShoppingCartItemAddedDTO } from '../../../models/DTO.model';
// import { ToastModule } from 'primeng/toast';
// import { MessageService } from 'primeng/api';
import { resolve } from 'path';
import { rejects } from 'assert';
import { MessageService } from 'primeng/api';
import { ToastService } from '../../../services/toast-service';
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
    private cartservices: ShoppingCart,
    private toastservice:ToastService,
  //  private cartService: CartService,
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
  }
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
}
