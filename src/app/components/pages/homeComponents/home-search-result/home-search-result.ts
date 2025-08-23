import { Component, Input, OnChanges, OnDestroy } from '@angular/core';
import { HomeSearchService } from '../../../../services/home-search-service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  ShoppingCartDto,
  ShoppingCartItemAddedDTO,
} from '../../../../models/DTO.model';
import { ShoppingCart } from '../../../../services/shoppingCart/shopping-cart';
import { ToastService } from '../../../../services/toast-service';
declare var bootstrap: any;
@Component({
  selector: 'app-home-search-result',
  imports: [RouterLink, CommonModule],
  templateUrl: './home-search-result.html',
  styleUrl: './home-search-result.css',
})
export class HomeSearchResult implements OnChanges, OnDestroy {
  @Input() searchTerm: string = '';

  private destroy$ = new Subject<void>();
  items: any[] = [];
  loading: boolean = false;
  hasError: boolean = false;

  constructor(
    private itemService: HomeSearchService,
    private router: Router,
    private cartservices: ShoppingCart,
    private toastservice: ToastService
  ) {}

  ngOnChanges() {
    if (this.searchTerm && this.searchTerm.trim() !== '') {
      this.searchItems();
    } else {
      this.items = [];
    }
  }

  ngOnInit() {
    this.itemService.testEndpoints(); // uncomment للاختبار
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  searchItems() {
    this.loading = true;
    this.hasError = false;

    this.itemService
      .search(this.searchTerm)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          const categoryItems = response.byCategory?.$values || [];
          const restaurantItems = response.byRestaurant?.$values || [];

          const allItems = [...categoryItems, ...restaurantItems];
          this.items = this.removeDuplicateItems(allItems);

          this.loading = false;
        },
        error: (error) => {
          console.error('Search Error:', error);
          console.error('Error Details:', error.error);
          console.error('Status:', error.status);
          console.error('Status Text:', error.statusText);

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
    const modalElement = document.getElementById('searchModal');
    if (modalElement) {
      const modal = bootstrap.Modal.getInstance(modalElement);
      if (modal) {
        modalElement.addEventListener(
          'hidden.bs.modal',
          () => {
            this.router.navigate(['/shoppingcart']);
          },
          { once: true }
        );

        modal.hide();
      } else {
        this.router.navigate(['/shoppingcart']);
      }
    } else {
      this.router.navigate(['/shoppingcart']);
    }
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

  async AddToCart(itemId: string, preferences: string): Promise<void> {
    const cart = await this.GetCart();
    if (cart?.cartID == null) {
      console.log('cannot get cart');
    } else {
      const cartitem: ShoppingCartItemAddedDTO = {
        itemID: itemId,
        cartID: cart?.cartID,
        preferences: preferences,
      };
      this.cartservices.addItemToCart(cartitem).subscribe({
        next: (res) => {
          console.log(res);
          this.toastservice.showSuccess(
            'Item Added To Shopping Cart Successfully',
            'Add To Cart'
          );
          this.GoToCart();
        },
        error: (err) => {
          console.log(err);
          if (err.status === 401) {
            this.toastservice.showError(
              'You need to signin or register before you can Add Items To cart',
              'Login needed'
            );
          } else if (err.status === 400)
            this.toastservice.showError(err.error, 'Add To Cart failed');
          else
            this.toastservice.showError(
              'failed to Add item To  shopping cart',
              'Add To Cart failed'
            );
        },
      });
    }
  }
}
