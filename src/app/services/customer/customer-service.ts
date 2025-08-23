import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { CustomerDto, OrderDetailDTO, OrderViewDTO, RegisterCustomerDTO, StatusEnum, UpdateCustomerDTO } from '../../models/DTO.model';
import { Observable } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { ApiResponse } from '../deliveryman.model';

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
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
    return this.http.get<CustomerDto>(`${this.basicUrl}Customer/ByID/${this.getuserId()}`,{headers:this.getAuthHeaders()});
  }
  updateCustomer(customer: UpdateCustomerDTO): Observable<any> {
    return this.http.put(`${this.basicUrl}Customer/UpdateCustomer?CustomerId=${this.getuserId()}`, customer, { headers: this.getAuthHeaders() });
  }
//orders

getallOrders(): Observable<{$id:string,$values:OrderViewDTO[]}> {
  return this.http.get<{$id:string,$values:OrderViewDTO[]}>(`${this.basicUrl}Order/AllOrdersForCustomer`, { headers: this.getAuthHeaders() });
}
getorderdetails(orderId: string): Observable<OrderDetailDTO> {
  return this.http.get<OrderDetailDTO>(`${this.basicUrl}Order/OrderDetailaForCustomer?orderId=${orderId}`, {
    headers: this.getAuthHeaders()})
  }
  getorderforcustomerbystatus(status: StatusEnum): Observable<{$id:string,$values:OrderViewDTO[]}> 
  {
    let params=new HttpParams()
    return this.http.request<{$id:string,$values:OrderViewDTO[]}>('GET',`${this.basicUrl}Order/OrderForCustomerbystatus?status=${encodeURIComponent(status)}`,{ headers: this.getAuthHeaders() });
  }

 
 //auth login
  private getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('authToken');

    if (token) {
      return new HttpHeaders({
        Authorization: `Bearer ${token}`,
      });
    }
    return new HttpHeaders(); // empty if no token
  }
  private getuserId():string{
    return sessionStorage.getItem('userId') ?? '';
    
  }
}
