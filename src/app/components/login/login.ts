import { Component, OnInit, Inject } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CustomValidators } from '../../services/validators.service';
import { AuthService as LoginService } from '../../services/auth';
import { Router } from '@angular/router';
import { loginResponse } from '../../models/ilogin';
import { NgIf } from '@angular/common';
import { PasswordModule } from 'primeng/password';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, FormsModule, PasswordModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
export class LoginComponent implements OnInit {
  value!: string;
  isLoading = false;
  errorMsg = '';
  error = '';
  baseUrl = 'http://localhost:5000'; // Add your backend base URL
  platformId: any; // Assign your platformId properly (if used)

  // Use constructor DI for all dependencies
  constructor(
    private loginAuth: LoginService,
    private router: Router,
    private http: HttpClient // Add http to constructor
  ) {}

  loginForm = new FormGroup({
    username: new FormControl('', [
      Validators.required,
      Validators.minLength(3),
    ]),
    password: new FormControl('', [
      Validators.required,
      CustomValidators.passwordValidator(),
    ]),
  });

  ngOnInit(): void {
    this.checkExistingSession();
  }

  checkExistingSession(): void {
    const token = sessionStorage.getItem('authToken');
    const role = sessionStorage.getItem('userRole');
    if (token && role === 'DeliveryMan') {
      console.log('User already logged in, redirecting...');
      this.router.navigate(['/DeliveryManDashboard']);
    }
  }

  submitForm(): void {
    if (this.loginForm.invalid) {
      this.errorMsg = 'Form is invalid';
      this.loginForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    this.errorMsg = '';
    const { username, password } = this.loginForm.value;
    this.loginAuth.login(username!, password!).subscribe({
      next: (response: loginResponse) => {
        this.isLoading = false;
        this.storeUserSession(response);
        this.redirectUser(response.role);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMsg = err.error?.Message || 'Login failed';
      },
    });
  }

  private storeUserSession(response: loginResponse): void {
    sessionStorage.setItem('authToken', response.token);
    sessionStorage.setItem('userId', response.userId);
    sessionStorage.setItem('userRole', response.role);
    sessionStorage.setItem('sessionId', response.$id);
    sessionStorage.setItem('loginTime', new Date().toISOString());
    sessionStorage.setItem(
      'userInfo',
      JSON.stringify({
        id: response.$id,
        userId: response.userId,
        role: response.role,
        loginTime: new Date().toISOString(),
      })
    );
  }

  redirectUser(role: string): void {
    switch (role.toLowerCase()) {
      case 'deliveryman': {
        const userId = this.loginAuth.getUserId();
        if (!userId) {
          this.error = 'User ID not found.';
          return;
        }
        const token = sessionStorage.getItem('authToken');
        if (!token) {
          this.error = 'Authorization token not found.';
          return;
        }

        if (!token) {
          this.error = 'Authorization token not found.';
          return;
        }
        const headers = new HttpHeaders({
          Authorization: `Bearer ${token}`,
        });
        this.http
          .get<{ accountStatus: number; deliveryManID: string; $id: string }>(
            `${this.baseUrl}/api/DeliveryMan/${userId}`,
            { headers }
          )
          .subscribe({
            next: (DeliveryMan) => {
              console.log('DeliveryMan response:', DeliveryMan);

              if (DeliveryMan.accountStatus === 1) {
                this.router
                  .navigate(['/DeliveryManDashboard'])
                  .catch((err) => console.error(err));
              } else {
                this.router.navigate(['/action-pending']);
              }
            },
            error: (err) => {
              console.error('Error fetching DeliveryMan info:', err);
              this.router.navigate(['/']);
            },
          });

        break;
      }

      case 'admin':
        const token = sessionStorage.getItem('authToken');
        if (token) {
          window.location.href = `http://localhost:5000/admin/Dashboard?token=${encodeURIComponent(
            token
          )}`;
        }
        break;
      case 'customer':
        this.router.navigate(['/CustomerDashboard']);
        break;
      case 'restaurant': {
        const userId = this.loginAuth.getUserId();
        console.log('userId', userId);
        if (!userId) {
          this.error = 'User ID not found.';
          return;
        }
        const token = sessionStorage.getItem('authToken');
        if (!token) {
          this.error = 'Authorization token not found.';
          return;
        }
        const headers = new HttpHeaders({
          Authorization: `Bearer ${token}`,
        });
        this.http
          .get<{ isActive: boolean }>(
            `${this.baseUrl}/api/restaurant/${userId}`,
            { headers }
          )

          .subscribe({
            next: (restaurant) => {
              console.log('restaurant', restaurant);
              if (restaurant.isActive) {
                this.router
                  .navigate(['/restaurant-dashboard'])
                  .catch((err) => console.error(err));
              } else {
                this.router.navigate(['/action-pending']);
              }
            },
            error: (err) => {
              console.error('Error fetching restaurant info:', err);
              this.router.navigate(['/']);
            },
          });
        break;
      }
      default:
        this.router.navigate(['/']);
        break;
    }
  }

  // Helper static methods
  static getStoredUserData(): any {
    const userInfo = sessionStorage.getItem('userInfo');
    return userInfo ? JSON.parse(userInfo) : null;
  }
  static isAuthenticated(): boolean {
    const token = sessionStorage.getItem('authToken');
    const userId = sessionStorage.getItem('userId');
    return !!(token && userId);
  }
  static getAuthToken(): string | null {
    return sessionStorage.getItem('authToken');
  }
  static getUserRole(): string | null {
    return sessionStorage.getItem('userRole');
  }
  static logout(): void {
    sessionStorage.removeItem('authToken');
    sessionStorage.removeItem('userId');
    sessionStorage.removeItem('userRole');
    sessionStorage.removeItem('sessionId');
    sessionStorage.removeItem('loginTime');
    sessionStorage.removeItem('userInfo');
    console.log('User session cleared');
  }

  signup() {
    this.router.navigate(['/customerRegister']);
  }
  forgotPassword() {
    this.router.navigate(['/reset-password']);
  }
}
