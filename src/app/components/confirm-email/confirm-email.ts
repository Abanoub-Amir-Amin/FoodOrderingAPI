import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  templateUrl: './confirm-email.html',
  imports: [NgClass],
})
export class ConfirmEmail implements OnInit {
  message: string = '';

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const userId = params['userId'];
      const token = params['token'];

      const apiUrl = 'http://localhost:5000/api/customer/confirm-email';
      const queryParams = new HttpParams()
        .set('userId', userId)
        .set('token', token);

      this.http.get(apiUrl, { params: queryParams, responseType: 'text' }).subscribe({
        next: (res) => this.message = res
      });
    });
  }
  home() {
    this.router.navigate(['/home']);
}
}
