import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUserSubject = new BehaviorSubject<any>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  private http = inject(HttpClient);
  private router = inject(Router);
  private baseUrl = 'http://localhost:5000/api';

  /***1-send (Username, Password) to => http://localhost:5000/api/auth/login  auth controller
   * 2-get response (Token,Role,UserId)
   * 3-store them in localstorage
   * 4-define a logout method(gust remove the response from database)
   * define function to get every part from response alone to used it as I want :) ==
   * ===> like Role in login to switch over it for different dashboard
  */
  login(Username: string, Password: string): Observable<any> {
    const url = `${this.baseUrl}/auth/login`;
    const payload = { username: Username, password: Password };

    return this.http.post<any>(url, payload).pipe(
      tap((res) => {
        console.log('Login response:', res);
        if (!res || !res.token) {
          throw new Error('No token in the response');
        }
        localStorage.setItem('Token', res.token);
        localStorage.setItem('Role', res.role);
        localStorage.setItem('UserId', res.userId);
        this.currentUserSubject.next(res);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('Token');
    localStorage.removeItem('Role');
    localStorage.removeItem('UserId');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('Token') || !!localStorage.getItem('token');
  }

  getUserRole(): string | null {
    return localStorage.getItem('Role') || localStorage.getItem('role');
  }

  getUserId(): string | null {
    return localStorage.getItem('UserId') || localStorage.getItem('userId');
  }
}
