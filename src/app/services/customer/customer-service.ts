import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { CustomerDto, OrderDetailDTO, OrderViewDTO, RegisterCustomerDTO, StatusEnum, UpdateCustomerDTO } from '../../models/DTO.model';
import { Observable } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { ApiResponse } from '../deliveryman.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  userid:string = '';  
  headers !: HttpHeaders;
  private apiUrl = 'http://localhost:5000/api/Customer/Register';

  private basicUrl= 'http://localhost:5000/api/';
  constructor(private http:HttpClient, @Inject(PLATFORM_ID) id:object){
    if(isPlatformBrowser(id)){
      this.headers = this.getAuthHeaders();
    }
    }
  
  register(customer: RegisterCustomerDTO): Observable<any> {
    return this.http.post(this.apiUrl, customer);
  }
  getCustomerById(): Observable<CustomerDto> {
    return this.http.get<CustomerDto>(`${this.basicUrl}Customer/ByID/${this.userid}`,{headers:this.headers});
  }
  updateCustomer(customer: UpdateCustomerDTO): Observable<any> {
    return this.http.put(`${this.basicUrl}Customer/UpdateCustomer?CustomerId=${this.userid}`, customer, { headers: this.headers });
  }
//orders

getallOrders(): Observable<OrderViewDTO[]> {
  return this.http.get<OrderViewDTO[]>(`${this.basicUrl}Order/AllOrdersForCustomer`, { headers: this.headers });
}
getorderdetails(orderId: string): Observable<OrderDetailDTO> {
  return this.http.get<OrderDetailDTO>(`${this.basicUrl}Order/AllOrdersForCustomer/?orderId=${orderId}`, {
    headers: this.headers})
  }
  getorderforcustomerbystatus(status: StatusEnum[]): Observable<OrderViewDTO[]> {
    return this.http.request<OrderViewDTO[]>('GET',`${this.basicUrl}Order/OrderForCustomerbystatus`, {body:{status}, headers: this.headers });
  }

 
 //auth login
  private getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('authToken');
    this.userid = sessionStorage.getItem('userId') ?? ''

    if (token) {
      return new HttpHeaders({
        Authorization: `Bearer ${token}`,
      });
    }
    return new HttpHeaders(); // empty if no token
  }
}
