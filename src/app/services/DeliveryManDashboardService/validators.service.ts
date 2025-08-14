import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class CustomValidators {
  // Password validator: must contain uppercase letter and special character
  static passwordValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }

      const hasUpperCase = /[A-Z]/.test(control.value);
      const hasSpecialChar = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(
        control.value
      );

      const errors: ValidationErrors = {};

      if (!hasUpperCase) {
        errors['noUpperCase'] = true;
      }

      if (!hasSpecialChar) {
        errors['noSpecialChar'] = true;
      }

      if (control.value.length < 8) {
        errors['minLength'] = true;
      }

      return Object.keys(errors).length > 0 ? errors : null;
    };
  }

  // Phone validator: must start with 01 and be 11 digits
  static phoneValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }

      const phonePattern = /^01\d{9}$/;

      if (!phonePattern.test(control.value)) {
        return { invalidPhone: true };
      }

      return null;
    };
  }

  // Email validator (enhanced)
  static emailValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }

      const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

      if (!emailPattern.test(control.value)) {
        return { invalidEmail: true };
      }

      return null;
    };
  }
}
