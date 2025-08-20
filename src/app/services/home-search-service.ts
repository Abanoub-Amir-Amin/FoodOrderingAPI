import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, of, map, catchError } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class HomeSearchService {
  private baseUrl = 'http://localhost:5000/api/Item/items';

  constructor(private http: HttpClient) {}


  getItemsByCategory(category: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/bycategory?category=${category}`);
  }


  getItemsByRestaurantName(restaurantName: string): Observable<any> {
    return this.http.get(
      `${this.baseUrl}/byrestaurantname?restaurantName=${restaurantName}`
    );
  }


  search(term: string): Observable<any> {
    return forkJoin({
      byCategory: this.getItemsByCategory(term),
      byRestaurant: this.getItemsByRestaurantName(term),
    });
  }
}
