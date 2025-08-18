import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { SnackbarService } from '../../../services/snackbar';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from './../../../services/auth';
import { MapComponent } from '../../shared/map-component/map-component';

@Component({
  selector: 'app-restaurant-profile',
  templateUrl: './restaurant-profile.html',
  styleUrls: ['./restaurant-profile.css'],
  standalone: true,
  imports: [
    MapComponent,
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    // Import map component module here if standalone or declare in your AppModule
  ],
})
export class RestaurantProfileComponent implements OnInit {
  profileForm!: FormGroup;
  isSubmitting = false;
  selectedImageFile: File | null = null;
  imagePreviewUrl: string | null = null;

  private http = inject(HttpClient);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private snackbar = inject(SnackbarService);
  private authService = inject(AuthService);

  private baseUrl = 'http://localhost:5000/api';

  ngOnInit(): void {
    const userId = this.authService.getUserId();
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }

    // Initialize the form including lat/lng
    this.profileForm = this.fb.group({
      restaurantName: ['', Validators.required],
      location: ['', Validators.required],
      openHours: [''],
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      isAvailable: [true],
      latitude: [null],       
      longitude: [null],
      delivaryPrice: [null],
      orderTime: [null],
      imageUrl: [''],
    });

    const headers = this.getAuthHeaders();

    this.http.get<any>(`${this.baseUrl}/restaurant/${userId}`, { headers }).subscribe({
      next: (data) => {
        console.log("returned data:",data);
        this.profileForm.patchValue({
          restaurantName: data.restaurantName || '',
          location: data.location || '',
          openHours: data.openHours || '',
          userName: data.user?.userName || '',
          email: data.user?.email || '',
          phoneNumber: data.user?.phoneNumber || '',
          isAvailable: data.isAvailable ?? true,
          latitude: data.latitude ?? null,
          longitude: data.longitude ?? null,
          delivaryPrice : data.delivaryPrice  ?? null,
          orderTime: data.orderTime ? data.orderTime.substring(0,5) : null,  // take "HH:mm"
          imageUrl: data.imageFile || '',
        });
      },
      error: () => {
        this.snackbar.show('Failed to load profile data. Please try again.');
      },
    });
  }

  /**
   * Handler Reacting to map component emitting selected address info,
   * updating the form controls for latitude, longitude, and optionally location string.
   */
  setAddress(addressInfo: { latitude: number, longitude: number, label?: string }): void {
    if (!addressInfo) return;
    this.profileForm.patchValue({
      latitude: addressInfo.latitude,
      longitude: addressInfo.longitude,
      //location: addressInfo.label || this.profileForm.get('location')?.value,
    });
  }

  onSubmit(): void {
  if (this.profileForm.invalid) {
    this.snackbar.show('Please fix errors in the form before submitting.');
    return;
  }

  this.isSubmitting = true;

  const restaurantId = this.authService.getUserId();
  if (!restaurantId) {
    this.isSubmitting = false;
    this.router.navigate(['/login']);
    return;
  }

  const dto = this.profileForm.value;
  console.log("dto:", dto);

  if (!dto.email) {
    this.snackbar.show('Email is missing in the form data.');
    this.isSubmitting = false;
    return;
  }

  const formData = new FormData();
  formData.append('restaurantName', dto.restaurantName);
  formData.append('location', dto.location);
  formData.append('openHours', dto.openHours || '00:00');
  formData.append('isAvailable', dto.isAvailable ? 'true' : 'false');

  if (dto.latitude !== null && dto.latitude !== undefined) {
    formData.append('latitude', (dto.latitude ?? 0).toString());
  }
  if (dto.longitude !== null && dto.longitude !== undefined) {
    formData.append('longitude', (dto.longitude ?? 0).toString());
  }
  if (dto.delivaryPrice !== null && dto.delivaryPrice !== undefined) {
    formData.append('delivaryPrice', (dto.delivaryPrice ?? 0).toString());
  }
  if (dto.orderTime !== null && dto.orderTime !== undefined) {
    // Add seconds to time string if it has only HH:mm
    let orderTimeValue = dto.orderTime;
    if (orderTimeValue && orderTimeValue.length === 5) {
      orderTimeValue += ':00';
    }
    formData.append('orderTime', orderTimeValue);
  }

  // User info nested under 'User' prefix
  formData.append('User.userName', dto.userName || '');
  formData.append('User.email', dto.email);
  formData.append('User.phoneNumber', dto.phoneNumber || '');

  if (this.selectedImageFile) {
    formData.append('logoFile', this.selectedImageFile);
    console.log('Appending new logo file:', this.selectedImageFile.name);
  } else {
    console.log('No new logo file selected, keeping existing image.');
  }

  console.log('Form Data entries:');
  formData.forEach((value, key) => console.log(key, value));

  const headers = this.getAuthHeaders();

  this.http.put(`${this.baseUrl}/restaurant/${restaurantId}/update-profile`, formData, { headers }).subscribe({
    next: () => {
      this.isSubmitting = false;
      this.snackbar.show('Profile updated successfully!');
    },
    error: (err) => {
      this.isSubmitting = false;

      // Log everything for debugging
      console.error('Update error:', err);

      if (err.error) {
        console.error('Error response body:', err.error);
        if (err.error.errors) {
          console.error('Validation errors:', err.error.errors);
        }
        if (err.error.details) {
          console.error('Error details:', err.error.details);
        }
      }

      const message =
        err.error?.error ||
        (Array.isArray(err.error?.errors) ? err.error.errors.join(', ') : null) ||
        'Error updating profile. Please try again.';

      this.snackbar.show(message);
    }
  });
}

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('Token') || localStorage.getItem('token');
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }

  // onFileSelected handler if you want image upload later
  // onFileSelected(event: Event): void {
  //   const input = event.target as HTMLInputElement;
  //   if (!input.files || input.files.length === 0) return;
  //   this.selectedImageFile = input.files[0];
  //   // ... handle preview if desired
  // }
}
