import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { MapComponent } from '../../shared/map-component/map-component';
import { AddressDto } from '../../../models/DTO.model';
import { PasswordModule } from 'primeng/password';

@Component({
  selector: 'app-restaurant-apply',
  templateUrl: './restaurant-apply.html',
  styleUrls: ['./restaurant-apply.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MapComponent, PasswordModule],
})
export class RestaurantApply implements OnInit {
  applyForm!: FormGroup;
  applying = false;
  success = false;
  errorMessage = '';
  location: string = '';

  latitude: number | null = null;
  longitude: number | null = null;

  logoFile: File | null = null;
  logoPreview: string | ArrayBuffer | null = null;

  showPassword = false;

  private http = inject(HttpClient);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private baseUrl = 'http://localhost:5000/api/restaurant';

  ngOnInit(): void {
    this.applyForm = this.fb.group({
      restaurantName: ['', Validators.required],
      openHours: [''],
      location: [''],
      orderTime: ['', Validators.required],  // string for HH:mm format
      delivaryPrice: ['', [Validators.required, Validators.pattern(/^\d+(\.\d{1,2})?$/)]], // decimal validation
      isAvailable: [true],
      user: this.fb.group({
        userName: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        phone: [''],
        password: ['', [Validators.required, Validators.minLength(6)]],
      }),
    });
  }

  onLogoFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.logoFile = input.files[0];
      const reader = new FileReader();
      reader.onload = () => (this.logoPreview = reader.result);
      reader.readAsDataURL(this.logoFile);
    } else {
      this.logoFile = null;
      this.logoPreview = null;
    }
  }

  onSubmit(): void {
    if (this.applyForm.invalid) {
      this.applyForm.markAllAsTouched();
      return;
    }
    this.applying = true;

    const formValue = this.applyForm.value;
    const formData = new FormData();

    formData.append('RestaurantName', formValue.restaurantName || '');
    formData.append('Location', formValue.location || '');
    formData.append('OpenHours', formValue.openHours || '');

    let orderTimeStr = formValue.orderTime;
    if (orderTimeStr && orderTimeStr.length === 5) {
      orderTimeStr += ':00'; // Add seconds if missing
    } else if (!orderTimeStr) {
      orderTimeStr = '00:00:00';
    }
    formData.append('orderTime', orderTimeStr);

    formData.append('delivaryPrice', formValue.delivaryPrice ? formValue.delivaryPrice.toString() : '0.00');

    formData.append('isAvailable', formValue.isAvailable ? 'true' : 'false');

    if (this.logoFile) {
      formData.append('logoFile', this.logoFile, this.logoFile.name);
    }

    formData.append('latitude', this.latitude !== null ? this.latitude.toString() : '0');
    formData.append('longitude', this.longitude !== null ? this.longitude.toString() : '0');

    // Nested user properties with dot notation for backend binding
    formData.append('user.userName', formValue.user.userName || '');
    formData.append('user.email', formValue.user.email || '');
    formData.append('user.phone', formValue.user.phone || '');
    formData.append('user.password', formValue.user.password || '');

    // Debug: log FormData entries
    console.log('FormData entries:');
    formData.forEach((value, key) => {
      console.log(key, value);
    });

    this.http.post(`${this.baseUrl}/apply`, formData).subscribe({
      next: (response) => {
        this.success = true;
        this.applying = false;
        setTimeout(() => this.router.navigate(['/action-pending']), 1500);
      },
      error: (error) => {
  console.error('Apply failed:', error);

  if (error?.error?.details) {
    console.error('Validation details:', error.error.details);
  }

  this.errorMessage =
    error?.error?.error ||
    (error?.error?.details ? error.error.details.join('; ') : '') ||
    error?.error?.message ||
    error?.message ||
    'An error occurred.';
  this.applying = false;
},
    });
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  setAddress(add: AddressDto) {
    this.latitude = add.latitude;
    this.longitude = add.longitude;
    // Optionally update location string display here
    // this.location = `${add.label} - ${add.street}, ${add.city}`;
  }
}
