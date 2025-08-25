import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthService } from '../../services/auth';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AvailabilityService {
  private apiUrl = 'http://prestoordering.somee.com/api/DeliveryMan/availability';

  private loginInfo = inject(AuthService);

  private availabilitySource = new BehaviorSubject<boolean>(false);

  availability$ = this.availabilitySource.asObservable();

  setAvailability(status: boolean) {
    this.availabilitySource.next(status);
  }

  constructor(private http: HttpClient) {}

  getAvailabilityStatus(): Observable<any> {
    return this.http
      .get<any>(this.apiUrl, {
        headers: this.loginInfo.getAuthHeaderDeleviry(),
      })
      .pipe(
        catchError((error) => {
          console.error('API Error:', error);
          throw error;
        })
      );
  }

  switchAvailabilityStatus(status: boolean): Observable<any> {
    return this.http
      .patch<any>(
        'http://prestoordering.somee.com/api/DeliveryMan/UpdateAvailability',
        status,
        { headers: this.loginInfo.getAuthHeaderDeleviry() }
      )
      .pipe(
        catchError((error) => {
          console.error('API Error:', error);
          throw error;
        })
      );
  }
}
