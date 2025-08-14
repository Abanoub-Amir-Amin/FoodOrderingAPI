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
    console.log('userId:', userId);
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }

    this.profileForm = this.fb.group({
      restaurantName: ['', Validators.required],
      location: ['', Validators.required],
      openHours: [''],
      userName: [''],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      logoFile: [''], // holds filename string only
      imageUrl: [''],
      isAvailable: [true],
    });

    const headers = this.getAuthHeaders();

    this.http.get<any>(`${this.baseUrl}/restaurant/${userId}`, { headers }).subscribe({
      next: (data) => {
        console.log('Loaded restaurant data:', data);
        this.profileForm.patchValue({
          restaurantName: data.restaurantName || '',
          location: data.location || '',
          openHours: data.openHours || '',
          userName: data.user.userName || '',
          email: data.user.email || '',
          phoneNumber: data.user.phoneNumber || '',
          logoFile: '', // clear file input display on patch
          isAvailable: data.isAvailable ?? true,
        });
        
        this.imagePreviewUrl = data.imageFile
          ? this.authService.getImageUrl(data.imageFile)
          : 'assets/restaurantLogo';
          console.log("this.imagePreviewUrl:", this.imagePreviewUrl);
      },
      error: () => {
        this.snackbar.show('Failed to load profile data. Please try again.');
      },
    });
  }

  // onFileSelected(event: Event): void {
  //   const input = event.target as HTMLInputElement;
  //   if (!input.files || input.files.length === 0) {
  //     return;
  //   }
  //   this.selectedImageFile = input.files[0];
  //   console.log('Selected file:', this.selectedImageFile);

  //   // Patch filename for display only
  //   this.profileForm.patchValue({ logoFile: this.selectedImageFile.name });

  //   // Preview selected image
  //   const reader = new FileReader();
  //   reader.onload = () => {
  //     this.imagePreviewUrl = reader.result as string;
  //     console.log('Image preview updated');
  //   };
  //   reader.readAsDataURL(this.selectedImageFile);
  // }

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
    console.log('Form DTO:', dto);

    if (!dto.email) {
      this.snackbar.show('Email is missing in the form data.');
      this.isSubmitting = false;
      return;
    }

    const formData = new FormData();
    formData.append('restaurantName', dto.restaurantName);
    formData.append('location', dto.location);
    formData.append('openHours', dto.openHours || '');
    formData.append('User.userName', dto.userName || '');
    formData.append('User.email', dto.email);
    formData.append('User.phoneNumber', dto.phoneNumber || '');

    if (this.selectedImageFile) {
      formData.append('logoFile', this.selectedImageFile);
      console.log('Appending file to formData:', this.selectedImageFile.name);
      console.log('Appending file to formData:', this.selectedImageFile);
    } else {
      console.log('No new image file selected, not appending logoFile.');
    }

    formData.append('isAvailable', dto.isAvailable ? 'true' : 'false');

    const headers = this.getAuthHeaders();

    this.http.put(`${this.baseUrl}/restaurant/${restaurantId}/update-profile`, formData, { headers }).subscribe({
      next: (response) => {
        console.log('Update response:', response);
        this.isSubmitting = false;
        this.snackbar.show('Profile updated successfully!');
      },
      error: (err) => {
        console.error('Update error:', err);
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
