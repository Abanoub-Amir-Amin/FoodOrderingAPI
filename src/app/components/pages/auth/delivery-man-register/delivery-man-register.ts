import {
  Component,
  OnInit,
  AfterViewInit,
  OnDestroy,
  Inject,
} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { PLATFORM_ID } from '@angular/core';
import { DeliverymanService } from '../../../../services/deliveryman.service';
import { CustomValidators } from '../../../../services/validators.service';
import { DeliveryManRegistration } from '../../../../services/deliveryman.model';

@Component({
  standalone: true,
  selector: 'app-delivery-man-register',
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './delivery-man-register.html',
  styleUrls: ['./delivery-man-register.css'],
})
export class DeliveryManRegister implements OnInit, AfterViewInit, OnDestroy {
  registrationForm!: FormGroup;
  showPassword = false;
  selectedLocation: { lat: number; lng: number } | null = null;
  map: any;
  marker: any;
  isSubmitting = false;
  formSubmitted = false;
  submitMessage = '';
  submitSuccess = false;

  hasUpperCase = false;
  hasSpecialChar = false;
  hasMinLength = false;

  constructor(
    private fb: FormBuilder,
    private deliverymanService: DeliverymanService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit() {
    this.initializeForm();
    this.setupPasswordValidation();
  }

  async ngAfterViewInit() {
    if (isPlatformBrowser(this.platformId)) {
      const L = await import('leaflet');

      this.map = L.map('map').setView([30.0444, 31.2357], 10);

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors',
      }).addTo(this.map);

      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
          (position) => {
            const lat = position.coords.latitude;
            const lng = position.coords.longitude;
            this.map.setView([lat, lng], 15);
            this.setLocationMarker(lat, lng);
          },
          (error) => {
            console.log('Geolocation error:', error);
          }
        );
      }

      this.map.on('click', (e: any) => {
        this.setLocationMarker(e.latlng.lat, e.latlng.lng);
      });
    }
  }

  ngOnDestroy() {
    if (this.map) {
      this.map.remove();
    }
  }

  initializeForm() {
    this.registrationForm = this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, CustomValidators.emailValidator()]],
      phoneNumber: [
        '',
        [Validators.required, CustomValidators.phoneValidator()],
      ],
      password: [
        '',
        [Validators.required, CustomValidators.passwordValidator()],
      ],
      agreeTerms: [false, [Validators.requiredTrue]],
    });
  }

  setupPasswordValidation() {
    this.registrationForm.get('password')?.valueChanges.subscribe((value) => {
      this.hasUpperCase = /[A-Z]/.test(value);
      this.hasSpecialChar = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(value);
      this.hasMinLength = value?.length >= 8;
    });
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

    this.selectedLocation = { lat, lng };
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.registrationForm.get(fieldName);
    return !!(
      field &&
      field.invalid &&
      (field.dirty || field.touched || this.formSubmitted)
    );
  }

  isFieldValid(fieldName: string): boolean {
    const field = this.registrationForm.get(fieldName);
    return !!(field && field.valid && (field.dirty || field.touched));
  }

  onSubmit() {
    this.formSubmitted = true;

    if (this.registrationForm.valid && this.selectedLocation) {
      this.isSubmitting = true;
      this.submitMessage = '';

      const registrationData: DeliveryManRegistration = {
        ...this.registrationForm.value,
        latitude: this.selectedLocation.lat,
        longitude: this.selectedLocation.lng,
      };

      this.deliverymanService.registerDeliveryman(registrationData).subscribe({
        next: (response) => {
          this.isSubmitting = false;
          this.submitSuccess = true;
          this.submitMessage =
            'Registration successful! Welcome to our delivery team.';
          this.registrationForm.reset();
          this.selectedLocation = null;
          if (this.marker) {
            this.map.removeLayer(this.marker);
          }
        },
        error: (error) => {
          this.isSubmitting = false;
          this.submitSuccess = false;
          this.submitMessage =
            error.error?.message || 'Registration failed. Please try again.';
        },
      });
    } else {
      this.submitMessage =
        'Please fill all required fields and select your location.';
      this.submitSuccess = false;
    }
  }
}
