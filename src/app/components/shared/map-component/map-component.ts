import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, Input, Output,EventEmitter } from '@angular/core';
import { PLATFORM_ID } from '@angular/core';
import { NgControl } from '@angular/forms';
// import { EventEmitter } from 'stream';
import { AddressDto } from '../../../models/DTO.model';
@Component({
  selector: 'app-map-component',
  imports: [CommonModule],
  templateUrl: './map-component.html',
  styleUrl: './map-component.css'
})
export class MapComponent {
   selectedLocation: { lat: number; lng: number } | null = null;
  map: any;
  marker: any;
  set:boolean = false;
  @Input() latitude: number | null = null;
  @Input() longitude: number | null = null;

  @Input() formSubmitted!: boolean;
  @Output() addressInfo = new EventEmitter<AddressDto>()
  @Output() mapReady = new EventEmitter<L.Map>();

  address:AddressDto| null = null;
  private isMapInitialized = false;

  constructor(
      @Inject(PLATFORM_ID) private platformId: Object
      
    ) {}
  async Intialize_Map() {
    debugger;
      if (isPlatformBrowser(this.platformId)) {
          const L = await import('leaflet');
           const container = L.DomUtil.get('map');
            if (container != null) {
              (container as any)._leaflet_id = null; // reset id
            }

          if(!this.map){
              this.map = await L.map('map').setView([30.0444, 31.2357], 10);

          L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors',
          }).addTo(this.map);

           const geocoderModule = await import('leaflet-control-geocoder');
          const geocoder = (L.Control as any).geocoder({
            defaultMarkGeocode: false
          });

          geocoder.on('markgeocode', (e: any) => {
            const latlng = e.geocode.center;
            this.setLocationMarker(latlng.lat, latlng.lng);
            this.map?.setView(latlng, 15);
          });
              geocoder.addTo(this.map);

          }
            this.isMapInitialized = true;
            this.mapReady.emit(this.map);


        }

}
 async ngAfterViewInit() {
    await this.Intialize_Map();
    if (this.latitude !== null && this.longitude !== null && this.map) {
      // Center map to input latitude and longitude if provided
      this.map.setView([this.latitude, this.longitude], 15);
      this.setLocationMarker(this.latitude, this.longitude);
    } else if (navigator.geolocation) {
      // fallback to user location
    if (this.latitude !== null && this.longitude !== null && this.map) {
      // Center map to input latitude and longitude if provided
      this.map.setView([this.latitude, this.longitude], 15);
      this.setLocationMarker(this.latitude, this.longitude);
    } else if (navigator.geolocation) {
      // fallback to user location
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const lat = position.coords.latitude;
          const lng = position.coords.longitude;
          if (this.map) {
            this.map.setView([lat, lng], 15);
            this.setLocationMarker(lat, lng);
            this.map.setView([lat, lng], 15);
            this.setLocationMarker(lat, lng);
          }
        },
        (error) => {
          console.log('Geolocation error:', error);
        }
      );
    }

    if (this.map) {
      this.map.on('click', (e: any) => {
        this.setLocationMarker(e.latlng.lat, e.latlng.lng);
      });
    }

    if (this.map) {
      this.map.on('click', (e: any) => {
        this.setLocationMarker(e.latlng.lat, e.latlng.lng);
      });
    }
  }
 }

  ngOnDestroy() {
     if (this.map) {
    this.map.off();  // remove all listeners
    this.map.remove();
    this.map = null;
    this.marker = null;
    this.isMapInitialized = false;
  }
  }
  async setLocationMarker(lat: number, lng: number) {
    const L = await import('leaflet');

    if (this.marker) {
      this.map.removeLayer(this.marker);
    }

    this.marker = L.marker([lat, lng], {
      icon: L.icon({
        iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
        shadowUrl:
          'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41],
      }),
    }).addTo(this.map);
    await this.reverseGeocode(lat, lng);    
    this.selectedLocation = { lat, lng };
    this.set = true;
    if(this.address)
      this.addressInfo.emit(this.address);
  }
  
async reverseGeocode(lat: number, lng: number) {
  // const L = await import('leaflet');
  // await import('leaflet-control-geocoder');
  // const geocoder = (L.Control as any).Geocoder.nominatim();
  // console.log(geocoder)
  // geocoder.reverse(L.latLng(lat,lng), this.map.getZoom(), (results: any) => {
  //    console.log('Raw reverse results:', results);

  //     if (results && results.length > 0) {
  //       console.log('Reverse geocode result:', results[0]);
  //     } else {
  //       console.log('No address found');
  //     }
  // });
try {
    const response = await fetch(
      `https://nominatim.openstreetmap.org/reverse?lat=${lat}&lon=${lng}&format=json`,
      {
        headers: {
          'Accept-Language': 'en',
        },
      }
    );

    const data = await response.json();

    if (data && data.address) {
      this.address = {
        label: data.address.amenity|| 'Apartment',
        street: (data.address.house_number||'')+ ' '+ (data.address.road || ''),
        city: data.address.city || data.address.town || '',
        latitude:lat,
        longitude:lng
      };
      
    } else {
      console.log('No address found');
    }
  } catch (err) {
    console.error('Reverse geocoding failed:', err);
  }
}
 }