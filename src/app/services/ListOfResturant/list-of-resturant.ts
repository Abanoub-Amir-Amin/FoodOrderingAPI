import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RestaurantItem, ResturantInterface } from '../../models/ResturantInterface/resturant-interface';

export interface Review {
  reviewID: string;
  orderID: string;
  customerID: string;
  userName: string;
  restaurantID: string;
  rating: number;
  comment: string;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class ListOfResturant {
  private apiUrl = 'http://prestoordering.somee.com/api/Restaurant/GetRestaurants';
  private itemsApiUrl = 'http://prestoordering.somee.com/api/Item/items/byrestaurantname';
  private reviewsApiUrl = 'http://prestoordering.somee.com/api/Review';

  constructor(private http: HttpClient) {}

  getAllRestaurants(): Observable<{ $values: ResturantInterface[] }> {
    return this.http.get<{ $values: ResturantInterface[] }>(this.apiUrl);
  }

  getRestaurantById(id: string): Observable<ResturantInterface> {
    return this.http.get<ResturantInterface>(`${this.apiUrl}/${id}`);
  }

  getItemsByRestaurantName(name: string): Observable<{ $values: RestaurantItem [] }> {
    return this.http.get<{ $values: RestaurantItem [] }>(`${this.itemsApiUrl}?restaurantName=${name}`);
  }

  // ✅ دالة جديدة تجيب كل الريفيوهات
  getAllReviews(): Observable<{ $values: Review[] }> {
    return this.http.get<{ $values: Review[] }>(this.reviewsApiUrl);
  }
}
