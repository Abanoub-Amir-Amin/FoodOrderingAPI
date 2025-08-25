import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { Observable } from 'rxjs';
import { AddressDto, AddressViewDto } from '../../models/DTO.model';
import { text } from 'stream/consumers';

@Injectable({
  providedIn: 'root'
})
export class AddressService {
  userid:string = '';  
  headers !: HttpHeaders;
  private basicUrl= 'http://prestoordering.somee.com/api/';
  constructor(private http:HttpClient, @Inject(PLATFORM_ID) id:object){
    if(isPlatformBrowser(id)){
      this.headers = this.getAuthHeaders();
    }
    }
  
    getalladdresses():Observable<{$id:string,$values:AddressViewDto[]}>{
      return this.http.get<{$id:string,$values:AddressViewDto[]}>(`${this.basicUrl}Address?UserId=${this.userid}`,{headers:this.headers})
    }
    getaddressbyid(AddressId:string):Observable<AddressViewDto>{
      return this.http.get<AddressViewDto>(`${this.basicUrl}Address/ById?AddressId=${AddressId}`,{headers:this.headers})
    }

    getdefaultaddress():Observable<AddressViewDto>{
      return this.http.get<AddressViewDto>(`${this.basicUrl}DefaultAddress?customerId=${this.userid}`,{headers:this.headers})
    }

    addAddress(address:AddressDto):Observable<AddressViewDto>{
      return this.http.post<AddressViewDto>(`${this.basicUrl}Address?userId=${this.userid}`,{
        label: address.label,
        street: address.street,
        city: address.city,
        latitude: address.latitude,
        longitude: address.longitude
      },{headers:this.headers})
    }

    updateAddress(addressId:string,address:AddressDto):Observable<AddressViewDto>{
      return this.http.put<AddressViewDto>(`${this.basicUrl}Address?AddressId=${addressId}`,{
        label: address.label,
        street: address.street,
        city: address.city,
        latitude: address.latitude,
        longitude: address.longitude
      },{headers:this.headers})
    }
    makeaddressDefault(AddressId:string):Observable<AddressViewDto>{
      return this.http.put<AddressViewDto>(`${this.basicUrl}Address/makeAddressDefault?AddressId=${AddressId}`,null,{headers:this.headers})
    }
    deleteAddress(AddressId:string):Observable<string>{
      return this.http.delete<string>(`${this.basicUrl}Address?AddressId=${AddressId}`,{headers:this.headers, responseType:'text' as 'json'});
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
