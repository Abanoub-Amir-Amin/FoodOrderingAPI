// src/app/core/services/cartService/cart-service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Order } from '../../models/order';


@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartItems = new BehaviorSubject<Order[]>([]);
  cartItems$ = this.cartItems.asObservable();

  addToCart(item: Order) {
    const items = this.cartItems.getValue();
    const existing = items.find(p => p.id === item.id);
    if (existing) {
      existing.quantity += item.quantity;
    } else {
      items.push({ ...item });
    }
    this.cartItems.next([...items]);
  }

  updateQuantity(id: number, quantity: number) {
    const items = this.cartItems.getValue().map(item =>
      item.id === id ? { ...item, quantity } : item
    );
    this.cartItems.next(items);
  }

  getTotalItems(): number {
    return this.cartItems.getValue().reduce((sum, item) => sum + item.quantity, 0);
  }

  getTotalPrice(): number {
    return this.cartItems.getValue().reduce((sum, item) => sum + item.price * item.quantity, 0);
  }
}
