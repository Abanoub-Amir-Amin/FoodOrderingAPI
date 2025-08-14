import {
  Component,
  OnInit,
  OnDestroy,
  AfterViewInit,
  Inject,
  PLATFORM_ID,
} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import {
  ProfileService,
  DeliveryManProfile,
} from '../../../services/DeliveryManDashboardService/profile-service';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css'],
})
export class Profile implements OnInit, OnDestroy, AfterViewInit {
  // Form data
  formData: DeliveryManProfile = {
    userName: '',
    email: '',
    phoneNumber: '',
    latitude: 29.302868696982962,
    longitude: 30.842443580613295,
  };

  // Form validation states
  formTouched = {
    userName: false,
    email: false,
    phoneNumber: false,
  };

  // Validation flags
  isValidUserName = true;
  isValidEmail = true;
  isValidPhone = true;

  // Error messages
  userNameError = '';
  emailError = '';
  phoneError = '';

  // Component states
  isLoading = false;
  isUpdating = false;
  errorMessage = '';
  successMessage = '';

  // Map related properties
  private map: any; // ← غيري النوع من L.Map لـ any
  private marker: any; // ← غيري النوع من L.Marker لـ any
  private L: any; // ← إضافي متغير للـ Leaflet

  private subscription = new Subscription();

  // Location properties
  currentLat = 29.302868696982962;
  currentLng = 30.842443580613295;
  isEditingLocation = false;

  private isBrowser: boolean;

  constructor(
    private profileService: ProfileService,
    private loginService: AuthService,
    private router: Router,
    @Inject(PLATFORM_ID) platformId: object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  ngOnInit(): void {
    // Check authentication
    if (
      !this.loginService.isAuthenticated() ||
      !this.loginService.hasRole('DeliveryMan')
    ) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadProfile();
  }

  ngAfterViewInit(): void {
    // Initialize map only on browser side after view init
    if (this.isBrowser) {
      setTimeout(() => {
        this.initializeMap();
      }, 100);
    }
  }

  private loadProfile(): void {
    this.isLoading = true;
    this.errorMessage = '';
    const profileSub = this.profileService.getProfile().subscribe({
      next: (profile: DeliveryManProfile) => {
        this.populateForm(profile);
        this.updateMapLocation(profile.latitude, profile.longitude);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading profile:', error);
        this.errorMessage = 'Failed to load profile data. Please try again.';
        this.isLoading = false;
      },
    });
    this.subscription.add(profileSub);
  }

  private populateForm(profile: DeliveryManProfile): void {
    this.currentLat = profile.latitude;
    this.currentLng = profile.longitude;
    this.formData = {
      userName: profile.userName,
      email: profile.email,
      phoneNumber: profile.phoneNumber,
      latitude: profile.latitude,
      longitude: profile.longitude,
    };
    // Validate all fields after loading
    this.validateAllFields();
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
        [this.currentLat, this.currentLng],
        13
      );

      // Add tile layer
      this.L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '© OpenStreetMap contributors',
      }).addTo(this.map);

      // Add marker
      this.marker = this.L.marker([this.currentLat, this.currentLng], {
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

  // Validation Methods

  validateUserName(): void {
    const value = this.formData.userName;
    if (!value || value.length < 2) {
      this.isValidUserName = false;
      this.userNameError = value
        ? 'User name must be at least 2 characters'
        : 'User name is required';
    } else {
      this.isValidUserName = true;
      this.userNameError = '';
    }
  }

  validateEmail(): void {
    const value = this.formData.email;
    if (!value) {
      this.isValidEmail = false;
      this.emailError = 'Email is required';
    } else {
      // Fixed regex: removed extra escape for '.'
      const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
      if (!emailPattern.test(value)) {
        this.isValidEmail = false;
        this.emailError = 'Please enter a valid email address';
      } else {
        this.isValidEmail = true;
        this.emailError = '';
      }
    }
  }

  validatePhone(): void {
    const value = this.formData.phoneNumber;
    if (!value) {
      this.isValidPhone = false;
      this.phoneError = 'Phone number is required';
    } else {
      const phonePattern = /^01\d{9}$/;
      if (!phonePattern.test(value)) {
        this.isValidPhone = false;
        this.phoneError = 'Phone must start with 01 and be 11 digits';
      } else {
        this.isValidPhone = true;
        this.phoneError = '';
      }
    }
  }

  validateAllFields(): void {
    this.validateUserName();
    this.validateEmail();
    this.validatePhone();
  }

  get isFormValid(): boolean {
    return this.isValidUserName && this.isValidEmail && this.isValidPhone;
  }

  onFieldBlur(fieldName: string): void {
    this.formTouched[fieldName as keyof typeof this.formTouched] = true;
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

  getCurrentLocation(): void {
    if (!this.isBrowser) {
      this.errorMessage = 'Geolocation is not supported in this environment.';
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
          this.errorMessage =
            'Unable to get current location. Please select manually.';
        }
      );
    } else {
      this.errorMessage = 'Geolocation is not supported by this browser.';
    }
  }

  private updateCoordinates(lat: number, lng: number): void {
    this.currentLat = lat;
    this.currentLng = lng;
    this.formData.latitude = lat;
    this.formData.longitude = lng;

    if (this.marker) {
      this.marker.setLatLng([lat, lng]);
    }
    if (this.map) {
      this.map.setView([lat, lng]);
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

  onSubmit(): void {
    this.validateAllFields();
    Object.keys(this.formTouched).forEach((key) => {
      this.formTouched[key as keyof typeof this.formTouched] = true;
    });

    if (this.isFormValid) {
      this.isUpdating = true;
      this.errorMessage = '';
      this.successMessage = '';

      const updateSub = this.profileService
        .updateProfile(this.formData)
        .subscribe({
          next: () => {
            this.successMessage = 'Profile updated successfully!';
            this.isUpdating = false;
            this.isEditingLocation = false;
            this.marker?.dragging?.disable();
          },
          error: (error) => {
            console.error('Update error:', error);
            this.errorMessage = 'Failed to update profile. Please try again.';
            this.isUpdating = false;
          },
        });

      this.subscription.add(updateSub);
    }
  }

  onCancel(): void {
    this.loadProfile();
    this.isEditingLocation = false;
    this.errorMessage = '';
    this.successMessage = '';
    Object.keys(this.formTouched).forEach((key) => {
      this.formTouched[key as keyof typeof this.formTouched] = false;
    });
    this.marker?.dragging?.disable();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
    if (this.map) {
      this.map.remove();
    }
  }
}
