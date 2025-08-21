import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { Router, NavigationError } from '@angular/router';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { HomeComponent } from './components/pages/home/home.component';
import { ToastModule } from "primeng/toast";
//import { AuthInterceptor } from './services/auth-token-interceptor';
// import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ToastModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
  // providers: [
  //   {
  //     provide: HTTP_INTERCEPTORS,
  //     useClass: AuthInterceptor,
  //     multi: true,
  //   },
  // ],
})
export class AppComponent {
  protected readonly title = signal('my-food-ordering-app');

  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationError) {
        console.error('Navigation Error:', event.error);
      }
    });
  }
}
