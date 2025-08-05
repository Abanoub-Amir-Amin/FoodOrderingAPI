import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
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
  //Reactive form group for login inputs: username and password with validation
  loginForm = new FormGroup({
    UserName: new FormControl('', [Validators.required]),
    Password: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });
//to store error messages and display in the template
  error = '';
showPassword=false;
  private http = inject(HttpClient);
  private router = inject(Router);
  //Backend base URL for API calls
  private baseUrl = 'http://localhost:5000/api';

  //Inject authentication service to handle login logic
  constructor(private auth: AuthService) {}

  onLogin() {
    console.log('onLogin() called');
    if (this.loginForm.invalid) {
      console.log('Form invalid');
      this.loginForm.markAllAsTouched();
      return;
    }

    //Extract username and password values from form
    const { UserName, Password } = this.loginForm.value;

    this.auth.login(UserName!, Password!).subscribe({
      next: () => {
        const role = this.auth.getUserRole();
        const normalizedRole = role?.toLowerCase();
        console.log('User role from storage:', role);

        if (!role) {
          this.error = 'User role not found.';
          return;
        }

        switch (normalizedRole) {
          case 'admin':
            window.location.href = 'http://localhost:5000/admin/dashboard';
            break;

          case 'restaurant': {
            //For restaurant ID from AuthService(auth.ts :) ) 
            const userId = this.auth.getUserId();
            console.log('UserId from storage:', userId);
            if (!userId) {
              this.error = 'User ID not found.';
              return;
            }

            //Get JWT token manually from localStorage
            const token = localStorage.getItem('Token') || localStorage.getItem('token');
            if (!token) {
              this.error = 'Authorization token not found.';
              return;
            }
            
            //create headers with Authorization Bearer with token which we got it from AuthService(auth.ts :)
            const headers = new HttpHeaders({
              Authorization: `Bearer ${token}`,
            });

            //call backend API to get restaurant info including isActive status with auth headers
            this.http
              .get<{ isActive: boolean }>(`${this.baseUrl}/restaurant/${userId}`, { headers })
              .subscribe({
                next: (restaurant) => {
                  //console.log ===> for deppuging reason :) (عشان طلع عيني والله)
                  console.log('Full restaurant response:', restaurant);
                  console.log('IsActive property:', restaurant.isActive);
                  //ىavigate based on restaurant account active status
                  if (restaurant.isActive) {
                    console.log('IsActive value:', restaurant.isActive, typeof restaurant.isActive);
                    this.router.navigate(['/dashboard']).catch((err) => console.error('Router navigation error:', err));
                  } else {
                    this.router.navigate(['/action-pending']);
                    console.log('IsActive value:', restaurant.isActive, typeof restaurant.isActive);
                  }
                },
                ////on error case, log it and navigate to home////
                error: (err) => {
                  console.error('Error fetching restaurant info:', err);
                  this.router.navigate(['/']);
                },
              });

            break;
          }

          // case 'customer':
          //   this.router.navigate('/customer-home');
          //   break;

          // case 'deliveryMan':
          //   this.router.navigate('/deliveryman-control-panel');
          //   break;


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
