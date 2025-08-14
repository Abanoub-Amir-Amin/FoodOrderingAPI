import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthService } from './auth';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  private baseUrl = 'http://localhost:5000/api';

  constructor(
    private auth: AuthService,
    private router: Router,
    private http: HttpClient
  ) {}
  /*****
   *guard prevents users who are not logged in from accessing protected pages. Without it, anyone could navigate directly by URL.
   *It checks if there's a logged-in user and a valid user ID in the client storage.
   *It calls your backend API to check if the restaurant account is active — allowing only active users to access certain routes.
    *====> If not logged in or user invalid → redirect to login page.
    *====> If logged in but account pending approval → redirect to a pending approval page.
    *====> If all good → allow route activation.
   */

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> {
    if (!this.auth.isLoggedIn()) {
      return of(this.router.createUrlTree(['/login']));
    }

    const userId = this.auth.getUserId();
    console.log("userId", userId);
    if (!userId) {
      this.auth.logout();
      return of(this.router.createUrlTree(['/login']));
    }

    return this.http.get<{ isActive: boolean }>(`${this.baseUrl}/restaurant/${userId}`).pipe(
      map(restaurant => {
        console.log(restaurant);
        if (restaurant && restaurant.isActive) {
          return true;
        } else {
          return this.router.createUrlTree(['/action-pending']);
        }
      }),
      catchError(() => of(this.router.createUrlTree(['/login'])))
    );
  }
}
