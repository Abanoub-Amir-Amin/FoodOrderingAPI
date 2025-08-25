import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, inject, Injectable, PLATFORM_ID } from '@angular/core';
import { map, Observable } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import {
  ShoppingCartDto,
  ShoppingCartItemAddedDTO,
} from '../../models/DTO.model';
import { ApiResponse } from '../deliveryman.model';

@Injectable({
  providedIn: 'root',
})
export class ShoppingCart {
  private apiUrl = 'http://prestoordering.somee.com/api/';
  headers!: HttpHeaders;
  constructor(private http: HttpClient, @Inject(PLATFORM_ID) id: object) {
    console.log('called...')
    if (isPlatformBrowser(id)) {
      this.headers = this.getAuthHeaders();
    }
  }
  GetCart(): Observable<ShoppingCartDto> {
    console.log(this.headers, 'userid', this.getuserId());
    return this.http.get<ShoppingCartDto>(
      `${this.apiUrl}ShoppingCart/ShoppingCart?customerid=${this.getuserId()}`,
      { headers: this.getAuthHeaders() }
    );
    //  .pipe(
    //   map(cart => ({
    //     ...cart,
    //     shoppingCartItems: (cart.shoppingCartItems?.$values || [] )as ShoppingCartDto[]
    //   }))
    // );
  }

  addItemToCart(cartitem: ShoppingCartItemAddedDTO): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(
      `${this.apiUrl}ShoppingCart/addItem`,
      {
        cartID: cartitem.cartID,
        itemID: cartitem.itemID,
        preferences: cartitem.preferences,
      },
      { headers: this.getAuthHeaders() }
    );
  }
  UpdateItemQuantity(cartitemId: string, addition: number): Observable<string> {
    console.log('headers', this.headers);
    return this.http.put<string>(
      `${this.apiUrl}ShoppingCart/UpdateItem?cartIemId=${cartitemId}&addition=${addition}`,
      null,
      { headers: this.getAuthHeaders(), responseType: 'text' as 'json' }
    );
  }
  DeleteItem(cartitemId: string): Observable<string> {
    return this.http.delete<string>(
      `${this.apiUrl}ShoppingCart/RemoveItem?cartIemId=${cartitemId}`,
      { headers: this.getAuthHeaders(), responseType: 'text' as 'json' }
    );
  }
  Clear(cartId: string): Observable<string> {
    return this.http.put<string>(
      `${this.apiUrl}ShoppingCart/Clear?cartid=${cartId}`,
      {
        cartid: cartId,
      },
      { headers: this.getAuthHeaders(), responseType: 'text' as 'json' }
    );
  }
  Checkout(): Observable<any> {
    // console.log(this.headers,"userid",this.userid)
    return this.http.get<any>(`${this.apiUrl}Order/Checkout`, {
      headers: this.getAuthHeaders(),
    });
  }
  placeOrder(SessionId:string):Observable<string>{
    return this.http.post<string>(`${this.apiUrl}Order/PlaceOrder?SessionId=${SessionId}`,{
    }, { headers: this.getAuthHeaders() , responseType: 'text' as 'json'  }
  );
  }
  private getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('authToken');
    console

    if (token) {
      return new HttpHeaders({
        Authorization: `Bearer ${token}`,
      });
    }
    return new HttpHeaders(); // empty if no token
  }
  private getuserId():string{
    return sessionStorage.getItem('userId') ?? '';
    
  }
}
