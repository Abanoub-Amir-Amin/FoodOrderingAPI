import { Component, inject, Inject, PLATFORM_ID } from '@angular/core';
import {
  FormControl,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { AuthService } from '../../services/auth';
import { Router } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
export class LoginComponent {
  loginForm = new FormGroup({
    UserName: new FormControl('', [Validators.required]),
    Password: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
    ]),
  });
  error = '';
  showPassword = false;

  private http = inject(HttpClient);
  private router = inject(Router);
  private baseUrl = 'http://localhost:5000/api';

  private auth = inject(AuthService);
  private platformId = inject(PLATFORM_ID);

  constructor() {}

  onLogin() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    const { UserName, Password } = this.loginForm.value;

    this.auth.login(UserName!, Password!).subscribe({
      next: () => {
        const role = this.auth.getUserRole();
        const normalizedRole = role?.toLowerCase();

        if (!role) {
          this.error = 'User role not found.';
          return;
        }

        if (!isPlatformBrowser(this.platformId)) {
          // If not a browser environment, do not run any window or router actions.
          this.error = 'Not running in browser environment.';
          return;
        }

        // Now safe to use window or router navigation
        switch (normalizedRole) {
          case 'admin':
            window.location.href = 'http://localhost:5000/admin/dashboard';
            break;

          case 'restaurant': {
            const userId = this.auth.getUserId();

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
                `${this.baseUrl}/restaurant/${userId}`,
                {
                  headers,
                }
              )
              .subscribe({
                next: (restaurant) => {
                  if (restaurant.isActive) {
                    this.router
                      .navigate(['/dashboard'])
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
          case 'deliveryman': {
            this.router.navigate(['/DeliveryManDashboard/']);
            break;
          }

          // Add other roles here as needed
          default:
            this.router.navigate(['/']);
            break;
        }
      },
      error: (err) => {
        this.error = 'Login failed. Check credentials.';
        console.error('Login error:', err);
      },
    });
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
}
