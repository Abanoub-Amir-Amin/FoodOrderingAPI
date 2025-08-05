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

@Component({
  selector: 'app-restaurant-profile',
  templateUrl: './restaurant-profile.html',
  styleUrls: ['./restaurant-profile.css'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
  ],
})
export class RestaurantProfileComponent implements OnInit {
  profileForm!: FormGroup;
  isSubmitting = false;

  private http = inject(HttpClient);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private snackbar = inject(SnackbarService);

  private baseUrl = 'http://localhost:5000/api';

  constructor() {}

  ngOnInit(): void {
    const userId = localStorage.getItem('userId');
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }

    this.profileForm = this.fb.group({
      restaurantName: ['', Validators.required],
      location: ['', Validators.required],
      openHours: [''],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      logoUrl: [''],
      isAvailable: [true],
    });

    const headers = this.getAuthHeaders();

    this.http.get<any>(`${this.baseUrl}/restaurant/${userId}`, { headers }).subscribe({
      next: (data) => {
        this.profileForm.patchValue({
          restaurantName: data.RestaurantName || '',
          location: data.Location || '',
          openHours: data.OpenHours || '',
          email: data.User?.Email || '',
          phone: data.User?.Phone || '',
          logoUrl: data.LogoUrl || '',
          isAvailable: data.IsAvailable ?? true,
        });
      },
      error: () => {
        this.snackbar.show('Failed to load profile data. Please try again.');
      },
    });
  }

  onSubmit(): void {
    if (this.profileForm.invalid) {
      this.snackbar.show('Please fix errors in the form before submitting.');
      return;
    }

    this.isSubmitting = true;

    const userId = localStorage.getItem('userId');
    if (!userId) return;

    const dto = this.profileForm.value;

    const formData = new FormData();
    formData.append('RestaurantName', dto.restaurantName);
    formData.append('Location', dto.location);
    formData.append('OpenHours', dto.openHours || '');
    formData.append('Email', dto.email);
    formData.append('Phone', dto.phone || '');
    formData.append('LogoUrl', dto.logoUrl || '');
    formData.append('IsAvailable', dto.isAvailable ? 'true' : 'false');

    const headers = this.getAuthHeaders();

    this.http.put(`${this.baseUrl}/restaurant/${userId}/update-profile`, formData, { headers }).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.snackbar.show('Profile updated successfully!');
      },
      error: () => {
        this.isSubmitting = false;
        this.snackbar.show('Error updating profile. Please try again.');
      },
    });
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('Token') || localStorage.getItem('token');
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }
}
