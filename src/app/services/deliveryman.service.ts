import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, DeliveryManRegistration } from './deliveryman.model';


@Injectable({
  providedIn: 'root',
})
export class DeliverymanService {
  private apiUrl = 'http://localhost:5000/api/DeliveryMan/apply';

  constructor(private http: HttpClient) {}

  registerDeliveryman(
    registrationData: DeliveryManRegistration
  ):
   Observable<ApiResponse> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.http.post<ApiResponse>(this.apiUrl, registrationData, {
      headers,
    });
  }
}
