import { CommonModule } from '@angular/common';
import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioButton } from '@angular/material/radio';
import { MainLayoutComponent } from '../../layout/main-layout/main-layout.component';
import { ResturanrtLogo } from '../../pages/resturanrt-logo/resturanrt-logo';
import { ShoppingCartDto } from '../../../models/DTO.model';
// import { MessageService } from 'primeng/api';
import { ShoppingCart as ShoppingCartService } from '../../../services/shoppingCart/shopping-cart';
// import { ToastModule } from 'primeng/toast';
import { firstValueFrom } from 'rxjs';
import { ToastService } from '../../../services/toast-service';
// import { MessageService } from 'primeng/api';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-shopping-cart',
  imports: [MatButtonModule, MatIconModule, CommonModule, MainLayoutComponent],
  providers: [],
  templateUrl: './shopping-cart.html',
  styleUrl: './shopping-cart.css',
})
export class ShoppingCart implements OnInit {
  cart: ShoppingCartDto | null = null;
  cartempty: boolean = true;
  loading = true;
  fullurl: string = '';

  constructor(
    private cartservices: ShoppingCartService,
    private toastservice: ToastService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}
  async ngOnInit(): Promise<void> {
    // debugger;
    try {
      this.cart = await this.GetCart();
      console.log(this.cart?.shoppingCartItems.$values);
    } catch (err) {
      console.log('error in getting data' + err);
    }
    if (!this.cart?.shoppingCartItems?.$values?.length) {
      this.cartempty = true;
    } else {
      this.cartempty = false;
    }
    console.log('cart empty' + this.cartempty);
  }
  Checkout() {
    this.cartservices.Checkout().subscribe({
      next: (res) => {
        console.log(res);
        this.toastservice.showSuccess('Checkout Successfully', 'Checkout');
        if (isPlatformBrowser(this.platformId)) {
          window.open(res.paymentLink, '_self');
        }
      },
      error: (err) => {
        console.log(err);
        if (err.status === 401) {
          this.toastservice.showError(
            'You need to signin or register before you can checkout',
            'Login needed'
          );
        } else if (err.status === 400) {
          this.toastservice.showError(err.error, 'Checkout failed');
        } else
          this.toastservice.showError('failed to checkout', 'Checkout failed');
      },
    });
  }
  GetCart(): Promise<ShoppingCartDto> {
    return new Promise((resolve, reject) => {
      this.cartservices.GetCart().subscribe({
        next: (res) => {
          this.loading = false;
          console.log(res?.cartID);
          resolve(res);
        },
        error: (err) => {
          console.log(err);

          if (err.status === 401) {
            this.toastservice.showError(
              'You need to signin or register before you can get cart',
              'Login needed'
            );
          } else if (err.status === 400)
            this.toastservice.showError(err.error, 'get Cart failed');
          else
            this.toastservice.showError(
              'failed to get shopping cart',
              'get Cart failed'
            );
          reject(err);
        },
      });
    });
  }
  async update_item(cartItemId: string, addition: number): Promise<void> {
    console.log('cart item id', cartItemId);
    try {
      const res = await firstValueFrom(
        this.cartservices.UpdateItemQuantity(cartItemId, addition)
      );
      console.log(res);
      // this.messageservice.add({severity:'success',summary:"Add To Cart",detail:"Item Added To Shopping Cart Successfully"})
      // this.toastservice.showSuccess("Item Updated To Shopping Cart Successfully","Update Cart")

      this.cart = await this.GetCart();
    } catch (err: any) {
      console.log(err);
      if (err.status === 401) {
        this.toastservice.showError(
          'You need to signin or register before you can update Items of cart',
          'Login needed'
        );
      } else if (err.status === 400)
        this.toastservice.showError(err.error, 'Update To Cart failed');
      else
        this.toastservice.showError(
          'failed to Update item Of shopping cart',
          'Update To Cart failed'
        );
    }
  }
  async delete_item(cartItemId: string, item: Element | null) {
    try {
      debugger;
      const res = await firstValueFrom(
        this.cartservices.DeleteItem(cartItemId)
      );
      console.log(res);
      // this.toastservice.showSuccess("Item Delete From Shopping Cart Successfully","Delete Cart","bottom-right")
      await item?.classList.add('deleting');
      this.cart = await this.GetCart();
      this.cartempty = !this.cart?.shoppingCartItems?.$values?.length;
    } catch (err: any) {
      console.log(err);
      if (err.status === 401) {
        this.toastservice.showError(
          'You need to signin or register before you can delete Items from cart',
          'Login needed'
        );
      } else if (err.status === 400)
        this.toastservice.showError(err.error, 'Delete Cart failed');
      else
        this.toastservice.showError(
          'failed to delete item Of shopping cart',
          'Delete Cart failed'
        );
    }
  }
  ClearCart() {
    if (this.cart) {
      this.cartservices.Clear(this.cart?.cartID || '').subscribe({
        next: (res) => {
          console.log(res);
          // this.toastservice.showSuccess("Cleared Shopping Cart Successfully","Clear Cart")
          this.GetCart();
        },
        error: (err) => {
          console.log(err);
          if (err.status === 401) {
            this.toastservice.showError(
              'You need to signin or register before you can clear cart',
              'Login needed'
            );
          } else if (err.status === 400)
            this.toastservice.showError(err.error, 'Cleared Cart failed');
          else
            this.toastservice.showError(
              'Cleared shopping cart',
              'Cleared Cart failed'
            );
        },
      });
      this.cartempty = true;
    }
  }
}
