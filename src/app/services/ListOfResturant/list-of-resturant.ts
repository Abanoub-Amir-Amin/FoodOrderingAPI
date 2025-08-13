import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { RestaurantItem, ResturantInterface } from '../../models/ResturantInterface/resturant-interface';

@Injectable({
  providedIn: 'root'
})
export class ListOfResturant {
 private apiUrl = 'http://localhost:5000/api/Restaurant/GetRestaurants';
private itemsApiUrl = 'http://localhost:5000/api/Item/items/byrestaurantname';

  constructor(private http: HttpClient) {}

  getAllRestaurants(): Observable<{ $values: ResturantInterface[] }> {
    return this.http.get<{ $values: ResturantInterface[] }>(this.apiUrl);
  }
  getRestaurantById(id: string): Observable<ResturantInterface> {
    return this.http.get<ResturantInterface>(`${this.apiUrl}/${id}`);
  }
  //  getRestaurantById(id: string): Observable<any> {
  //   return this.http.get<any>(`${this.apiUrl}/${id}`);
  // }

  getItemsByRestaurantName(name: string): Observable<{ $values: RestaurantItem[] }> {
    return this.http.get<{ $values: RestaurantItem[] }>(`${this.itemsApiUrl}?restaurantName=${name}`);
  }
}
