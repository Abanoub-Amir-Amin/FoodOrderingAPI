import { Component, Inject, OnInit, PLATFORM_ID, TemplateRef, ViewChild, ViewContainerRef } from '@angular/core';
import { AddressDto, AddressViewDto } from '../../../models/DTO.model';
import { AddressService } from '../../../services/address/address-service';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MapComponent } from '../../shared/map-component/map-component';
import { Overlay, OverlayModule, OverlayRef } from '@angular/cdk/overlay';
import { PortalModule, TemplatePortal } from '@angular/cdk/portal';
// import { Modal } from 'bootstrap';

@Component({
  selector: 'app-customer-addresses',
  imports: [CommonModule,MapComponent,OverlayModule,PortalModule],
  templateUrl: './customer-addresses.html',
  styleUrl: './customer-addresses.css'
})
export class CustomerAddresses implements OnInit {
// private overlay = Inject(Overlay);
private overlayRef?: OverlayRef;

@ViewChild('addAddressModal') addAddressModal!: TemplateRef<any>;
@ViewChild('mapModal') mapModal!: TemplateRef<any>;


AddressesView:AddressViewDto[] = [];
loading = true;
ErrorMessage = '';
successMessage = '';
bootstrap:any

//for add map model
addmap!: L.Map;
isAdded:boolean =false;
isAddModalOpen = false;
isMapModalOpen = false;
addsucessMessage='';
adderrorMessage='';

//for map model to edit model
selectedAddress: AddressDto ={
label:"",
street:"",
city:"",
latitude:0,
longitude:0
};
AddedAddress: AddressDto ={
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
currentLat:number=0
set:boolean = false;
currentLng:number=0
MaperrorMessage:string=""
selectedAddId:string = '';
constructor(
  private overlay: Overlay,
  private vcr: ViewContainerRef,
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
viewOnMap(AddressId:string,label:string,street:string,city:string,lat:number,lng:number){
  this.selectedAddId = AddressId;
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
   
    // // Initialize map after modal is shown
    if (this.isBrowser) {
      setTimeout(() => {
        this.initializeMap();
      }, 300);
    }
 this.openModal('map')
}

closeViewMap(){
  // debugger;
//  const model = document.getElementById('mapModal');
      // if (model){
        this.selectedAddress = {
          label: '', 
          street: '',
          city: '',
          latitude: 0,
          longitude: 0
        };
    // this.isMapModalOpen = false;
    this.set = false;
    // this.toggleBodyScroll(true);
    
    // Clean up map if exists
    if (this.map) {
      this.map.remove();
      this.map = null;
      this.marker = null;
    }
  // document.body.style.overflow = ''; // Add this line
  this.closeModal();
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
UpdateAddress(){
  this.addressService.updateAddress(this.selectedAddId,this.selectedAddress).subscribe({
    next:(res)=>{
      this.successMessage="update this address successfully"
      this.ErrorMessage=""
      console.log(res)
      this.getAddresses();

    },
    error:(err)=> {
    this.ErrorMessage="Failed update This Address"
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
      await this.updateCoordinates(this.selectedAddress.latitude, this.selectedAddress.longitude);
      // Handle marker drag end event
      this.marker.on('dragend', async (e: any) => {
        const position = e.target.getLatLng();
        await this.updateCoordinates(position.lat, position.lng);
      });

      // Map click event (update only if editing mode enabled)
      this.map.on('click', async (e: any) => {
        if (this.isEditingLocation) {
          await this.updateCoordinates(e.latlng.lat, e.latlng.lng);
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
 async getCurrentLocation(): Promise<void> {
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

  private async updateCoordinates(lat: number, lng: number): Promise<void> {
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
    await this.reverseGeocode(lat, lng)
  }

 updateLocationMode(): void {
  debugger;
    this.isEditingLocation = !this.isEditingLocation;
    if (this.marker) {
      if (this.isEditingLocation) {
        this.marker.dragging?.enable();
        this.successMessage = 'Click on map or drag marker to update location';
      } else {
        this.UpdateAddress()
        this.marker.dragging?.disable();
        this.successMessage = '';
      }
    }
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
      this.selectedAddress = {
        label: data.address.amenity|| 'Apartment',
        street: (data.address.house_number||'')+ ' '+ (data.address.road || ''),
        city: data.address.city || data.address.town || '',
        latitude:lat,
        longitude:lng
      };
      if(this.selectedAddress)
        this.set=true
      
    } else {
      console.log('No address found');
    }
  } catch (err) {
    console.error('Reverse geocoding failed:', err);
  }
}

  // UTILITY: Toggle body scroll for modal
  private toggleBodyScroll(enable: boolean) {
    if (this.isBrowser) {
      if (enable) {
        document.body.classList.remove('modal-open');
        document.body.style.overflow = '';
        document.body.style.paddingRight = '';
      } else {
        document.body.classList.add('modal-open');
        document.body.style.overflow = 'hidden';
      }
    }
  }
//add map
// AddonMap(){
//   debugger;
//    console.log('Opening add modal...');
// ; 
//    this.isAddModalOpen = true;
//     this.toggleBodyScroll(false);
//     this.isAdded = false;
    
//  setTimeout(() => {
//       if (this.addmap) {
//         this.addmap.invalidateSize();
//       }
//     }, 300);
    
//   document.body.style.overflow = 'hidden'; // Add this line
  

// }
  onMapReady(map: L.Map) {
    this.addmap = map;
  }
closeAddMap(){
    // debugger;
  this.AddedAddress = {
    label: '', 
    street: '',
    city: '',
    latitude: 0,
    longitude: 0
  };
  this.isAdded=false
  this.isAddModalOpen = false;
  this.toggleBodyScroll(true);
    document.body.style.overflow = ''; // Add this line

}
setAddress(add:AddressDto) {
    this.AddedAddress = add;
  }
AddAddress(){
  this.addressService.addAddress(this.AddedAddress).subscribe({
    next:(res)=>{
      this.successMessage="Add this address successfully"
      this.ErrorMessage=""
      console.log(res)
      this.getAddresses();
      this.isAdded=true
    },
    error:(err)=> {
    this.ErrorMessage="Failed Adding This Address"
      console.log(err);
      this.successMessage=""
    },
  })
}
 onBackdropClick(event: Event, modalType: 'add' | 'map') {
    if (event.target === event.currentTarget) {
      if (modalType === 'add') {
        this.closeAddMap();
      } else {
        this.closeViewMap();
      }
    }
  }
   openModal(templateName: 'add' | 'map') {
        const modalTemplate = templateName === 'add' ? this.addAddressModal : this.mapModal;

if (!modalTemplate) {
      console.warn(`${templateName} not ready yet`);
      return;
    }

    if (!this.overlayRef) {
      this.overlayRef = this.overlay.create({
        hasBackdrop: true,
        backdropClass: 'modal',
        panelClass: 'modal-container',
        scrollStrategy: this.overlay.scrollStrategies.block()
      });

      this.overlayRef.backdropClick().subscribe(() => this.closeModal());
    }

    if (!this.overlayRef.hasAttached()) {
      const portal = new TemplatePortal(modalTemplate, this.vcr);
      this.overlayRef.attach(portal);
    }
  }

  // قفل المودال
  closeModal() {
    if (this.overlayRef) {
      this.overlayRef.dispose();
      this.overlayRef = undefined;
    }
  }
}
