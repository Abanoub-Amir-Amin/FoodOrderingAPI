import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DeliveryManProfile, ProfileService } from '../../../services/DeliveryManDashboardService/profile-service';
import { AuthService } from '../../../services/auth';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { CustomerDto, GenderEnum, UpdateCustomerDTO } from '../../../models/DTO.model';
import { CustomerService } from '../../../services/customer/customer-service';
import { SelectionModel } from '@angular/cdk/collections';

@Component({
  selector: 'app-customer-profile',
  imports: [CommonModule,FormsModule],
  templateUrl: './customer-profile.html',
  styleUrl: './customer-profile.css'
})
export class CustomerProfile {
  formData: CustomerDto = {
      firstName:'',
      lastName:'',
      userName: '',
      email: '',
      phone: '',
      gender:null,
      addresses:[],
      totalOrders:0
    };
  
    // Form validation states
    formTouched = {
      phone: false,
      firstName:false,
      lastName:false
    };
  
    // Validation flags
    isValidPhone = true;
    isValidFname=true;
    isValidLname=true;

    // Error messages
    phoneError = '';
    fnameError='';
    lnameError='';

    // Component states
    isLoading = false;
    isUpdating = false;
    errorMessage = '';
    successMessage = '';
  
    
    private subscription = new Subscription();
  
  
    private isBrowser: boolean;
  
    constructor(
      private profileService: CustomerService,
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
        !this.loginService.hasRole('Customer')
      ) {
        this.router.navigate(['/login']);
        return;
      }
      this.loadProfile();
    }
  
  
    private loadProfile(): void {
      this.isLoading = true;
      this.errorMessage = '';
      const profileSub = this.profileService.getCustomerById().subscribe({
        next: (profile) => {
          console.log('Profile loaded:', profile);
          this.populateForm(profile);
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
  
    private populateForm(profile:any): void {

      this.formData = {
        firstName: profile.firstName,
        lastName: profile.lastName,
        userName: profile.userName,
        email: profile.email,
        phone: profile.phoneNumber,
        gender: profile.gender,
        addresses: profile.addresses || [],
        totalOrders: profile.totalOrders || 0,
      };
      // Validate all fields after loading
      this.validateAllFields();
    }
  
    // Validation Methods
  
  
    validatePhone(): void {
      const value = this.formData.phone;
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
    validateFname(){
      const value=this.formData.firstName;
      if(!value)
      {
        this.isValidFname = false;
        this.fnameError = 'First Name is required';
      }
      else if(value.length<2)
      {
        this.isValidFname = false;
        this.fnameError = 'First Name must be at least 2 characters';
      }
      
      else{
          this.isValidFname = true;
          this.fnameError = '';
      }
    }
    validateLname(){
      const value=this.formData.lastName;
      if(!value)
      {
        this.isValidLname = false;
        this.lnameError = 'Last Name number is required';
      }
      
      else if(value.length<2)
      {
        this.isValidFname = false;
        this.fnameError = 'Last Name must be at least 2 characters';
      }
      else{
          this.isValidLname = true;
          this.lnameError = '';
      }
    }
    validateAllFields(): void {
      this.validatePhone();
      this.validateFname();
      this.validateLname();
    }
  
    get isFormValid(): boolean {
      return  this.isValidPhone && this.isValidFname && this.isValidLname;
    }
  
    onFieldBlur(fieldName: string): void {
      this.formTouched[fieldName as keyof typeof this.formTouched] = true;
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
        const updateData: UpdateCustomerDTO={
          FirstName: this.formData.firstName,
          LastName: this.formData.lastName,
          Gender: this.formData.gender,
          PhoneNumber: this.formData.phone,
        }
        const updateSub = this.profileService
          .updateCustomer(updateData)
          .subscribe({
            next: () => {
              this.successMessage = 'Profile updated successfully!';
              this.isUpdating = false;
              // this.isEditingLocation = false;
              // this.marker?.dragging?.disable();
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
      // this.isEditingLocation = false;
      this.errorMessage = '';
      this.successMessage = '';
      Object.keys(this.formTouched).forEach((key) => {
        this.formTouched[key as keyof typeof this.formTouched] = false;
      });
      // this.marker?.dragging?.disable();
    }
  
    ngOnDestroy(): void {
      this.subscription.unsubscribe();
      // if (this.map) {
      //   this.map.remove();
      }
}
