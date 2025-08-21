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
  isRegestered = false;
  showPassword = false;
  address: AddressDto | null = null;
  private fb = inject(FormBuilder);
  private customerService = inject(CustomerService);

  // تم تحديث النموذج ليشمل الـ validator المخصص
  // وحذف حقل 'gender' لأنه غير موجود في الـ DTO
  registerForm = this.fb.group(
    {
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [
        '',
        [Validators.required, Validators.pattern(/^01[0-9]{9}$/)],
      ],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
    },
    { validators: passwordMatchValidator }
  ); // إضافة الـ validator على مستوى النموذج

  constructor(private router: Router) {}

  onSubmit() {
    // التحقق من صلاحية النموذج بالكامل، بما في ذلك تطابق كلمات المرور
    if (this.registerForm.valid && this.address) {
      const customer = this.registerForm.getRawValue() as RegisterCustomerDTO;
      customer.Address = this.address;
      this.customerService.register(customer).subscribe({
        next: () => {
          // يُفضل استخدام شريط تنبيه أو رسالة في الواجهة بدلًا من alert()
          alert('Customer registered successfully');
          this.isRegestered = true;
          this.registerForm.reset();
          this.router.navigate(['/login']);
        },
        error: (err) => {
          console.error('Registration failed', err);
          if (err.status === 400) {
            if (err.error.errors) {
              alert(
                'Registration failed. Please check your input and try again.\n' +
                  err.error.title
              );
            }
            if (err.error['Creation error']) {
              alert(
                'Registration failed. Please check your input and try again.\n' +
                  err.error['Creation error'][0]
              );
            }
          } else
            alert(
              'Registration failed. Please check your input and try again.\n'
            );
        },
      });
    } else {
      if (!this.address) {
        alert('Please select an address on the map.');
      } else {
        // إذا كان النموذج غير صالح، يتم التنبيه
        alert(
          'Form is invalid. Please fill all required fields and ensure passwords match.'
        );
        // يمكن استخدام console.log(this.registerForm.errors) لمعرفة سبب عدم الصلاحية
      }
    }
  }
  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
  setAddress(add: AddressDto) {
    this.address = add;
  }
}
