import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, forkJoin, of, map, catchError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class HomeSearchService {
  private baseUrl = 'http://prestoordering.somee.com/api/Item';

  constructor(private http: HttpClient) {}

  getItemsByCategory(category: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/items`).pipe(
      map((response: any) => {
        const items = response.$values || [];
        const filteredItems = items.filter(
          (item: any) =>
            item.category?.toLowerCase().includes(category.toLowerCase()) ||
            item.name?.toLowerCase().includes(category.toLowerCase()) ||
            item.description?.toLowerCase().includes(category.toLowerCase())
        );
        return { $values: filteredItems };
      }),
      catchError((error) => {
        console.error('Category search error:', error);
        return of({ $values: [] });
      })
    );
  }

  getItemsByRestaurantName(restaurantName: string): Observable<any> {
    return this.http
      .get(
        `${
          this.baseUrl
        }/items/byrestaurantname?restaurantName=${encodeURIComponent(
          restaurantName
        )}`
      )
      .pipe(
        catchError((error) => {
          console.error('Restaurant search error:', error);
          return of({ $values: [] });
        })
      );
  }

  search(term: string): Observable<any> {
    const searchTerm = term.trim();

    if (!searchTerm) {
      return of({ byCategory: { $values: [] }, byRestaurant: { $values: [] } });
    }

    return forkJoin({
      byCategory: this.getItemsByCategory(searchTerm),
      byRestaurant: this.getItemsByRestaurantName(searchTerm),
    }).pipe(
      catchError((error) => {
        console.error('Search service error:', error);
        return of({
          byCategory: { $values: [] },
          byRestaurant: { $values: [] },
        });
      })
    );
  }

  // أضيفي الـ method دي للاختبار
  testEndpoints(): void {
    console.log('Testing endpoints...');

    // اختبار 1: جيبي كل الـ items
    this.http.get(`${this.baseUrl}/items`).subscribe({
      next: (response) => console.log('All items:', response),
      error: (error) => console.error('All items error:', error),
    });

    // اختبار 2: البحث بالـ restaurant
    this.http
      .get(`${this.baseUrl}/items/byrestaurantname?restaurantName=kfc`)
      .subscribe({
        next: (response) => console.log('Restaurant search:', response),
        error: (error) => console.error('Restaurant search error:', error),
      });
  }
}
