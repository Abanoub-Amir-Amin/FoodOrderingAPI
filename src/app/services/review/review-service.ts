// src/app/services/review.service.ts
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { Observable } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';

export interface ReviewDTO {
  reviewId?: string;
  customerId: string;

  orderId: string;
  rating: number;
  comment: string;
  createdAt?: Date;
}

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private apiUrl = 'http://localhost:5000/api/Review';
  headers!: HttpHeaders;

  constructor(private http: HttpClient, @Inject(PLATFORM_ID) private platformId: object) {
    if (isPlatformBrowser(platformId)) {
      this.headers = this.getAuthHeaders();
    }
  }

  private getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('authToken');
    if (token) {
      return new HttpHeaders({
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      });
    }
    return new HttpHeaders({ 'Content-Type': 'application/json' });
  }

  // POST - create review
  createReview(review: ReviewDTO): Observable<any> {
    return this.http.post(this.apiUrl, review, { headers: this.headers });
  }

  // GET - all reviews
  getAllReviews(): Observable<ReviewDTO[]> {
    return this.http.get<ReviewDTO[]>(this.apiUrl, { headers: this.headers });
  }

  // GET - by review id
  getReviewById(reviewId: string): Observable<ReviewDTO> {
    return this.http.get<ReviewDTO>(`${this.apiUrl}/${reviewId}`, { headers: this.headers });
  }

  // ✅ GET - reviews by order id (orderId in route)
  getReviewsByOrderId(orderId: string): Observable<ReviewDTO[]> {
    return this.http.get<ReviewDTO[]>(`${this.apiUrl}/getorderreviewbyorderid/${orderId}`, { headers: this.headers });
  }

  // ✅ GET - reviews by customer id (customerId in route)
  getReviewsByCustomerId(customerId: string): Observable<ReviewDTO[]> {
    return this.http.get<ReviewDTO[]>(`${this.apiUrl}/getorderreviewbycustid/${customerId}`, { headers: this.headers });
  }

  // ✅ GET - reviews by restaurant id (restaurantId in route)
  getReviewsByRestaurantId(restaurantId: string): Observable<ReviewDTO[]> {
    return this.http.get<ReviewDTO[]>(`${this.apiUrl}/getorderreviewbyrestid/${restaurantId}`, { headers: this.headers });
  }

  // DELETE - review
  deleteReview(reviewId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${reviewId}`);
  }
}
