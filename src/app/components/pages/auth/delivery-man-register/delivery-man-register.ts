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
import { PasswordModule } from 'primeng/password';

@Component({
  standalone: true,
  selector: 'app-delivery-man-register',
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule, PasswordModule],
  templateUrl: './delivery-man-register.html',
  styleUrls: ['./delivery-man-register.css'],
})
export class DeliveryManRegister implements OnInit, AfterViewInit, OnDestroy {
  registrationForm!: FormGroup;
  showPassword = false;
  selectedLocation: { lat: number; lng: number } | null = null;
  map: any;
  marker: any;
  L: any; // ← إضافة متغير للـ Leaflet
  isSubmitting = false;
  formSubmitted = false;
  submitMessage = '';
  submitSuccess = false;
  isMapInitialized = false; // ← إضافة flag للتأكد من تهيئة الخريطة

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
      try {
        this.L = await import('leaflet');

        // إصلاح مشكلة الأيقونات
        delete (this.L.Icon.Default.prototype as any)._getIconUrl;
        this.L.Icon.Default.mergeOptions({
          iconRetinaUrl:
            'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
          iconUrl:
            'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
          shadowUrl:
            'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
          iconSize: [25, 41],
          iconAnchor: [12, 41],
          popupAnchor: [1, -34],
          shadowSize: [41, 41],
        });

        // التأكد من وجود العنصر قبل تهيئة الخريطة
        const mapElement = document.getElementById('map');
        if (!mapElement) {
          console.error('Map container element not found');
          return;
        }

        this.map = this.L.map('map').setView([30.0444, 31.2357], 10);

        this.L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
          attribution: '© OpenStreetMap contributors',
        }).addTo(this.map);

        this.isMapInitialized = true;

        // محاولة الحصول على الموقع الحالي
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

        // التعامل مع النقر على الخريطة
        this.map.on('click', (e: any) => {
          this.setLocationMarker(e.latlng.lat, e.latlng.lng);
        });
      } catch (error) {
        console.error('Error initializing map:', error);
      }
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

  setLocationMarker(lat: number, lng: number) {
    if (!this.L || !this.isMapInitialized) {
      console.error('Map or Leaflet not initialized');
      return;
    }

    // إزالة الـ marker السابق
    if (this.marker) {
      this.map.removeLayer(this.marker);
    }

    // إنشاء marker جديد
    this.marker = this.L.marker([lat, lng]).addTo(this.map);

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
          if (this.marker && this.map) {
            this.map.removeLayer(this.marker);
            this.marker = null;
          }
          this.formSubmitted = false;
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
