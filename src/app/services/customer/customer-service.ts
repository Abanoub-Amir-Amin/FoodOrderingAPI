import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterCustomerDTO } from '../../models/DTO.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  
 private apiUrl = 'http://localhost:5000/api/Customer/Register';


  constructor(private http: HttpClient) {}

  register(customer: RegisterCustomerDTO): Observable<any> {
    return this.http.post(this.apiUrl, customer);
  }
}
