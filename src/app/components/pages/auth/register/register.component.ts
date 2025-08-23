import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  Validators,
  ReactiveFormsModule,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

import { MainLayoutComponent } from '../../../layout/main-layout/main-layout.component';
import { AddressDto, RegisterCustomerDTO } from '../../../../models/DTO.model';
import { CustomerService } from '../../../../services/customer/customer-service';
import { MapComponent } from '../../../shared/map-component/map-component';
import { PasswordModule } from 'primeng/password';
import { CustomValidators } from '../../../../services/validators.service';

// validator مخصص للتحقق من تطابق كلمتي المرور
export const passwordMatchValidator = (
  control: AbstractControl
): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');


  // إذا لم يكن هناك قيم أو كانت متطابقة، لا توجد مشكلة
  if (
    !password ||
    !confirmPassword ||
    password.value === confirmPassword.value
  ) {
    return null;
  }

  // إذا كانت غير متطابقة، أعد خطأ
  return { passwordMismatch: true };
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink, MapComponent, PasswordModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  hasUpperCase = true;
  hasSpecialChar = true;
  hasMinLength = true;
  isRegestered = false;
  showPassword = false;
  address: AddressDto | null = null;
  private fb = inject(FormBuilder);
  private customerService = inject(CustomerService);
  submitMessage = '';
  submitSucess=false

  // تم تحديث النموذج ليشمل الـ validator المخصص
  // وحذف حقل 'gender' لأنه غير موجود في الـ DTO
  registerForm = this.fb.group(
    {
      firstName: ['',[ Validators.required,Validators.minLength(2)]],
      lastName: ['', [Validators.required,Validators.minLength(2)]],
      userName: ['', [Validators.required,Validators.minLength(3)]],
      email: ['', [Validators.required, CustomValidators.emailValidator]],
      phoneNumber: [
        '',
        [Validators.required, CustomValidators.phoneValidator],
      ],
      password: ['', [Validators.required, CustomValidators.passwordValidator]],
      confirmPassword: ['', Validators.required],
    },
    { validators: passwordMatchValidator }
  ); // إضافة الـ validator على مستوى النموذج

  constructor(private router: Router) {}
    ngOnInit() {
    this.setupPasswordValidation();
  }

  onSubmit() {
    if (this.registerForm.valid && this.address) {
          this.submitMessage = ''; 
      const customer = this.registerForm.getRawValue() as RegisterCustomerDTO;
      customer.Address = this.address;
      this.customerService.register(customer).subscribe({
        next: () => {
          this.submitMessage =
            'Registration successful! Welcome to Prezto.';
          this.submitSucess=true
          this.isRegestered = true;
          this.registerForm.reset();
          this.router.navigate(['/login']);
        },
        error: (err) => {
          console.error('Registration failed', err);
          if (err.status === 400) {
            if (err.error.errors) {
              this.submitMessage=
                  err.error.title
                  this.submitSucess=false
            }
            if (err.error['Creation error']) {
          this.submitMessage = err.error['Creation error'][0]

            }
          } else
            
              this.submitMessage='Registration failed. Please check your input and try again.\n'
        },
      });
    } else {
      if (!this.address) {
             } else {
        this.submitMessage =
        'Please fill all required fields and select your location.';
       
      }
    }
  }
  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
  setAddress(add: AddressDto) {
    this.address = add;
  }
   isFieldInvalid(fieldName: string): boolean {
    const field = this.registerForm.get(fieldName);
    return !!(
      field &&
      field.invalid &&
      (field.dirty || field.touched || this.isRegestered)
    );
  }

  isFieldValid(fieldName: string): boolean {
    const field = this.registerForm.get(fieldName);
    return !!(field && field.valid && (field.dirty || field.touched));
  }
setupPasswordValidation() {
    this.registerForm.get('password')?.valueChanges.subscribe((value) => {
      this.hasUpperCase = /[A-Z]/.test(value ?? '');
      this.hasSpecialChar = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(value ?? '');
      this.hasMinLength = (value ?? '').length >= 8;
    });
  }

}
