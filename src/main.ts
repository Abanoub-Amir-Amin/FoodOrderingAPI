import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app/app';
import { routes } from './app/app.routes';
import { provideAnimations } from '@angular/platform-browser/animations';
import { MessageService } from 'primeng/api';
// import { MessageService } from 'primeng/api';
// import { importProvidersFrom } from '@angular/core';
// import { ToastModule } from 'primeng/toast';


bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(), 
    provideRouter(routes),
    provideAnimations(),
    MessageService
  ],
}).catch(err => console.error(err));
