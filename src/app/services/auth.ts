import { Injectable, inject, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUserSubject = new BehaviorSubject<any>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  isLoading = true;
  restaurant: any = null;

  private isBrowser: boolean;
  private http = inject(HttpClient);
  private router = inject(Router);
  private baseUrl = 'http://localhost:5000/api';

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.isBrowser = isPlatformBrowser(this.platformId);

    // Optionally initialize currentUserSubject from sessionStorage on browser
    if (this.isBrowser) {
      const userInfo = sessionStorage.getItem('userInfo');
      if (userInfo) {
        this.currentUserSubject.next(JSON.parse(userInfo));
      }
    }
  }

  private getAuthHeaders(): HttpHeaders {
    if (!this.isBrowser) return new HttpHeaders();

    const token = sessionStorage.getItem('authToken');
    console.log('Token Info', token);
    return token
      ? new HttpHeaders({ Authorization: `Bearer ${token}` })
      : new HttpHeaders();
  }

  // Check if user is authenticated
  isAuthenticated(): boolean {
    const token = sessionStorage.getItem('authToken');
    const userId = sessionStorage.getItem('userId');
    return !!(token && userId);
  }

  login(username: string, password: string): Observable<any> {
    const url = `${this.baseUrl}/auth/login`;
    const payload = { username, password };

    return this.http.post<any>(url, payload).pipe(
      tap((res) => {
        console.log('Login response:', res);
        if (!res || !res.token) {
          throw new Error('No token in the response');
        }
        // Save to sessionStorage instead of localStorage
        sessionStorage.setItem('authToken', res.token);
        sessionStorage.setItem('userRole', res.role);
        sessionStorage.setItem('userId', res.userId);
        sessionStorage.setItem('sessionId', res.$id ?? ''); // Safe fallback
        sessionStorage.setItem('loginTime', new Date().toISOString());

        // Save userInfo object for quick reference
        sessionStorage.setItem(
          'userInfo',
          JSON.stringify({
            id: res.$id,
            userId: res.userId,
            role: res.role,
            loginTime: new Date().toISOString(),
          })
        );

        this.currentUserSubject.next(res);
      })
    );
  }
  rejectUser(){
    if (!this.isBrowser) return;

    sessionStorage.removeItem('authToken');
    sessionStorage.removeItem('userRole');
    sessionStorage.removeItem('userId');
    sessionStorage.removeItem('sessionId');
    sessionStorage.removeItem('loginTime');
    sessionStorage.removeItem('userInfo');
    this.currentUserSubject.next(null);
    
  }
  logout(): void {
    if (!this.isBrowser) return;

    sessionStorage.removeItem('authToken');
    sessionStorage.removeItem('userRole');
    sessionStorage.removeItem('userId');
    sessionStorage.removeItem('sessionId');
    sessionStorage.removeItem('loginTime');
    sessionStorage.removeItem('userInfo');
    this.currentUserSubject.next(null);
    this.router.navigate(['/home']);
  }

  isLoggedIn(): boolean {
    if (!this.isBrowser) {
      return false;
    }
    const token = !!sessionStorage.getItem('authToken');
    return token;
  }

  // Get stored auth token
  getAuthToken(): string | null {
    return sessionStorage.getItem('authToken');
  }

  getUserRole(): string | null {
    if (!this.isBrowser) return null;
    return sessionStorage.getItem('userRole');
  }

  // Check if user has specific role
  hasRole(role: string): boolean {
    const userRole = this.getUserRole();
    return userRole?.toLowerCase() === role.toLowerCase();
  }
  getUserId(): string | null {
    if (!this.isBrowser) return null;
    return sessionStorage.getItem('userId');
  }

  getUserIdChatbot(): string | null {
    if (typeof window === 'undefined' || !sessionStorage) {
      return null;
    }
    return sessionStorage.getItem('userId');
  }

  isLoggedInChatbot(): boolean {
    return this.getUserId() !== null;
  }

  loadRestaurant(id: string | null): Observable<any> {
    if (!id) {
      return new Observable((observer) => {
        observer.next(null);
        observer.complete();
      });
    }

    const headers = this.getAuthHeaders();
    this.isLoading = true;

    return this.http
      .get<any>(`${this.baseUrl}/restaurant/${id}`, { headers })
      .pipe(
        tap({
          next: (data) => {
            this.restaurant = data;
            this.isLoading = false;
          },
          error: (err) => {
            console.error('Error loading restaurant:', err);
            this.restaurant = null;
            this.isLoading = false;
          },
        })
      );
  }

  getImageUrl(imageFile: string | undefined | null): string {
    if (!imageFile) return 'assets/restaurantLogo.jpg';
    if (imageFile.startsWith('http')) return imageFile; // full URL
    return `http://localhost:5000${imageFile}`; // prefix backend server URL
  }

  // Check if token is expired (you'll need to decode JWT for this)
  isTokenExpired(): boolean {
    const token = this.getAuthToken();
    if (!token) return true;

    try {
      // Decode JWT payload
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Math.floor(Date.now() / 1000);

      return payload.exp < currentTime;
    } catch (error) {
      console.error('Error decoding token:', error);
      return true;
    }
  }

  // Get HTTP headers with auth token
  getAuthHeaderDeleviry(): HttpHeaders {
    const token = this.getAuthToken();
    return new HttpHeaders({
      Authorization: token ? `Bearer ${token}` : '',
      'Content-Type': 'application/json',
    });
  }
}
