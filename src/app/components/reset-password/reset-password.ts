import { Component, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-reset-password',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './reset-password.html',
})
export class ResetPassword {
  successMessage: string = '';
  errorMessage: string = '';

  form: any;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private cd: ChangeDetectorRef, // ✅ Inject ChangeDetectorRef
    private router: Router
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  submit() {
    if (this.form.invalid) return;

    const email = this.form.value.email;
    this.successMessage = '';
    this.errorMessage = '';

    this.http.post('http://prestoordering.somee.com/api/auth/forgot-password', { email }, { responseType: 'text' })
      .subscribe({
        next: (res) => {
          this.successMessage = res;
          this.errorMessage = '';
          this.cd.detectChanges(); // ✅ Ensure the view updates
        },
        error: (err) => {
          this.errorMessage = typeof err.error === 'string' ? err.error : 'Something went wrong.';
          this.successMessage = '';
          this.cd.detectChanges(); // ✅ Ensure the view updates
        }
      });
  }
  home() {
    this.router.navigate(['/home']);
}
}
