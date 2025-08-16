import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { AddressDto, AddressViewDto } from '../../../models/DTO.model';
import { AddressService } from '../../../services/address/address-service';
import { CommonModule, isPlatformBrowser } from '@angular/common';
// import { Modal } from 'bootstrap';

@Component({
  selector: 'app-customer-addresses',
  imports: [CommonModule],
  templateUrl: './customer-addresses.html',
  styleUrl: './customer-addresses.css'
})
export class CustomerAddresses implements OnInit {
AddressesView:AddressViewDto[] = [];
loading = true;
ErrorMessage = '';
successMessage = '';
bootstrap:any

//for map model to edit model
selectedAddress: AddressDto ={
label:"",
street:"",
city:"",
latitude:0,
longitude:0
};
isEditingLocation=false;
private map: any; // ← غيري النوع من L.Map لـ any
private marker: any; // ← غيري النوع من L.Marker لـ any
private L: any; // ← إضافي متغير للـ Leaflet
private isBrowser:boolean
private currentLat:number=0
private currentLng:number=0
MaperrorMessage:string=""

constructor(
  private addressService: AddressService, 
   @Inject(PLATFORM_ID) platformId: object
)
 { 
      this.isBrowser = isPlatformBrowser(platformId);
      

}
async ngOnInit(): Promise<void> {
  this.getAddresses();
  if(this.isBrowser)
  {
    const bs = await import('bootstrap');
    this.bootstrap = bs;
  }
}
getAddresses() {
  this.loading = true;
  this.addressService.getalladdresses().subscribe({
    next: (res) => {
      this.AddressesView = res.$values||[];
      this.loading = false;
      if (this.AddressesView.length === 0) {
        this.ErrorMessage = 'No addresses found.';
      } else {
        this.ErrorMessage = '';
      }
    },
    error: (err) => {
      console.error('Error fetching addresses:', err);
      this.ErrorMessage = 'Failed to load addresses. Please try again later.';
      this.loading = false;
      this.successMessage=""

    }
  });
}

MakeDefault(AddressId:string){
  this.addressService.makeaddressDefault(AddressId).subscribe({
    next:(res)=>{
      this.ErrorMessage=""
      console.log(res)
      this.getAddresses();
    },
    error:(err)=>{
      this.ErrorMessage="Failed Make This Address Default"
      console.log(err);
      this.successMessage=""

    }
  })
}
viewOnMap(label:string,street:string,city:string,lat:number,lng:number){

  this.selectedAddress = {
    label: label,
    street: street,
    city: city,
    latitude: lat,
    longitude: lng
  };
  this.isEditingLocation=false
    // Initialize map only on browser side after view init
    
  // Logic to open a modal or navigate to a map view can be added here
  console.log(`Viewing on map at Latitude: ${lat}, Longitude: ${lng}`);
  if(this.isBrowser){
    if(this.bootstrap){
      const model = document.getElementById('mapModal');
  if (model){
    new this.bootstrap.Modal(model,{backdrop:'static',keyboard:false}).show();
    setTimeout(() => {
      this.initializeMap();
    }, 100);
  }
  else
    console.error('Map modal element not found');
}
}
}
DeleteAddress(AddressId:string){
  this.addressService.deleteAddress(AddressId).subscribe({
    next:(res)=>{
      this.successMessage="deleted this address successfully"
      this.ErrorMessage=""
      console.log(res)
      this.getAddresses();
    },
    error:(err)=> {
    this.ErrorMessage="Failed Delete This Address"
      console.log(err);
      this.successMessage=""
    },
  })
}
 private async initializeMap(): Promise<void> {
    try {
      // Dynamic import للـ Leaflet
      this.L = await import('leaflet');

      // Defensive check in case container is missing
      const mapContainer = document.getElementById('profileMap');
      if (!mapContainer) {
        console.error('Map container element not found: profileMap');
        return;
      }

      // إصلاح مشكلة الأيقونات
      delete (this.L.Icon.Default.prototype as any)._getIconUrl;
      this.L.Icon.Default.mergeOptions({
        iconRetinaUrl:
          'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
        iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
        shadowUrl:
          'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41],
      });

      // Initialize map
      this.map = this.L.map('profileMap').setView(
        [this.selectedAddress?.latitude, this.selectedAddress?.longitude],
        13
      );

      // Add tile layer
      this.L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '© OpenStreetMap contributors',
      }).addTo(this.map);

      // Add marker
      this.marker = this.L.marker([this.selectedAddress?.latitude, this.selectedAddress?.longitude], {
        draggable: true,
      }).addTo(this.map);

      // Initially disable dragging unless editing location
      if (!this.isEditingLocation) {
        this.marker.dragging?.disable();
      }

      // Handle marker drag end event
      this.marker.on('dragend', (e: any) => {
        const position = e.target.getLatLng();
        this.updateCoordinates(position.lat, position.lng);
      });

      // Handle map click event - update location only if editing mode enabled
      this.map.on('click', (e: any) => {
        if (this.isEditingLocation) {
          this.updateCoordinates(e.latlng.lat, e.latlng.lng);
        }
      });
    } catch (error) {
      console.error('Error initializing map:', error);
    }
  }
  private updateMapLocation(lat: number, lng: number): void {
    if (this.map && this.marker) {
      this.map.setView([lat, lng]);
      this.marker.setLatLng([lat, lng]);
      this.currentLat = lat;
      this.currentLng = lng;
    }
  }
 getCurrentLocation(): void {
    if (!this.isBrowser) {
      this.MaperrorMessage = 'Geolocation is not supported in this environment.';
      return;
    }

    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const lat = position.coords.latitude;
          const lng = position.coords.longitude;
          this.updateCoordinates(lat, lng);
          this.updateMapLocation(lat, lng);
        },
        (error) => {
          console.error('Geolocation error:', error);
          this.MaperrorMessage =
            'Unable to get current location. Please select manually.';
        }
      );
    } else {
      this.MaperrorMessage = 'Geolocation is not supported by this browser.';
    }
  }

  private updateCoordinates(lat: number, lng: number): void {
    this.currentLat = lat;
    this.currentLng = lng;
    this.selectedAddress.latitude = lat;
    this.selectedAddress.longitude = lng;

    if (this.marker) {
      this.marker.setLatLng([lat, lng]);
    }
    if (this.map) {
      this.map.setView([lat, lng]);
    }
  }

 updateLocationMode(): void {
    this.isEditingLocation = !this.isEditingLocation;
    if (this.marker) {
      if (this.isEditingLocation) {
        this.marker.dragging?.enable();
        this.successMessage = 'Click on map or drag marker to update location';
      } else {
        this.marker.dragging?.disable();
        this.successMessage = '';
      }
    }
  }

}
