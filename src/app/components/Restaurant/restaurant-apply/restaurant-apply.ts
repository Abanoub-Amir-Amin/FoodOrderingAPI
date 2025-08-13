import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { MapComponent } from '../../shared/map-component/map-component';
import { AddressDto } from '../../../models/DTO.model';

@Component({
  selector: 'app-restaurant-apply',
  templateUrl: './restaurant-apply.html',
  styleUrls: ['./restaurant-apply.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,MapComponent],
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

  private baseUrl = 'http://localhost:5000/api';

  ngOnInit(): void {
    this.applyForm = this.fb.group({
    RestaurantName: ['', Validators.required],
    // Location: ['', Validators.required],
    OpenHours: [''],
    OrderTime:['',Validators.required],
    DelivaryPrice:['',Validators.required],
    IsAvailable: [true],
    UserName: ['', Validators.required],
    Email: ['', [Validators.required, Validators.email]],
    Phone: [''],
    Password: ['', [Validators.required, Validators.minLength(6)]],
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

    // Append flat Restaurant fields
    formData.append('RestaurantName', formValue.RestaurantName);
    if(this.location){
      formData.append('Location', this.location);
    }
    formData.append('OpenHours', formValue.OpenHours || '');
    formData.append('orderTime', formValue.OrderTime || '');
    formData.append('DelivaryPrice', formValue.DelivaryPrice || '');
    formData.append('IsAvailable', formValue.IsAvailable ? 'true' : 'false');

    if (this.logoFile) {
      formData.append('logoFile', this.logoFile, this.logoFile.name);
    }
    if(this.latitude){
      formData.append('Latitude', this.latitude?.toString() || '');
    }
    if(this.longitude){
      formData.append('Longitude', this.longitude?.toString() || '');
    }

    // Append nested User fields using dot notation (per ASP.NET Core conventions)
    formData.append('User.UserName', formValue.UserName);
    formData.append('User.Email', formValue.Email);
    formData.append('User.Phone', formValue.Phone || '');
    formData.append('User.Password', formValue.Password);

    this.http.post(`${this.baseUrl}/restaurant/apply`, formData).subscribe({
      next: (response) => {
        this.success = true;
        this.applying = false;
        setTimeout(() => this.router.navigate(['/action-pending']), 1500);
      },
      error: (error) => {
        console.error('Apply failed:', error);
        this.errorMessage = error?.error?.error || error.error?.message || error.message || 'An error occurred.';
        this.applying = false;
      },
    });
  }
   togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
  setAddress(add:AddressDto) {
      this.latitude = add.latitude;
      this.longitude = add.longitude;
      this.location = add.label + ' - ' + add.street + ', ' + add.city;
    }
}
