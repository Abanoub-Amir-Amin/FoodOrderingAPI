import { ChangeDetectorRef, Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { PasswordModule } from 'primeng/password';

@Component({
  selector: 'app-newpassword',
  imports: [ReactiveFormsModule, CommonModule, PasswordModule],
  templateUrl: './newpassword.html',
  styleUrl: './newpassword.css',
  encapsulation: ViewEncapsulation.None
})
export class NewPassword {
  
  successMessage: string = '';
  errorMessage: string = '';
  token: string = '';
  email: string = '';
  form: any;
  showPassword: boolean = false;
  showConfirmPassword: boolean = false;


  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private route: ActivatedRoute,
    private cd: ChangeDetectorRef,
    private router: Router
  ) {
      this.form = this.fb.group({
      newPassword: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#_-])[A-Za-z\d@$!%*?&#_-]+$/)
      ]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordsMatch });
}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      this.email = params['email'];
    });
  }

  passwordsMatch(group: AbstractControl) {
    const pass = group.get('newPassword')?.value;
    const confirm = group.get('confirmPassword')?.value;
    return pass === confirm ? null : { mismatch: true };
  }
  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }
  submit() {
    if (this.form.invalid) return;

    const newPassword = this.form.value['newPassword'];

    this.http.post('http://localhost:5000/api/auth/reset-password', {
      email: this.email,
      token: this.token,
      newPassword
    }, { responseType: 'text' }).subscribe({
      next: (res) => {
          this.successMessage = res;
          this.errorMessage = '';
          this.cd.detectChanges();
        },
        error: (err) => {
          this.errorMessage = typeof err.error === 'string' ? err.error : 'Something went wrong.';
          this.successMessage = '';
          this.cd.detectChanges();
        }
    });
  }
  home() {
    this.router.navigate(['/home']);
}
}