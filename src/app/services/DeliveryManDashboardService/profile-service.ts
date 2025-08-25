import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../../services/auth';

export interface DeliveryManProfile {
  $id?: string;
  userName: string;
  latitude: number;
  longitude: number;
  email: string;
  phoneNumber: string;
}

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private apiUrl = 'http://prestoordering.somee.com/api/DeliveryMan/profile';

  constructor(private http: HttpClient, private loginService: AuthService) {}

  private getHeaders(): HttpHeaders {
    const token = this.loginService.getAuthToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
  }

  // GET Profile Data
  getProfile(): Observable<DeliveryManProfile> {
    return this.http
      .get<DeliveryManProfile>(this.apiUrl, {
        headers: this.getHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  // UPDATE Profile Data
  updateProfile(profileData: DeliveryManProfile): Observable<any> {
    return this.http
      .put(this.apiUrl, profileData, {
        headers: this.getHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('Profile Service Error:', error);
    return throwError(() => error);
  }
}
