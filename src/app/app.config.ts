import { ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient,  withFetch, withInterceptorsFromDi } from '@angular/common/http';

import { routes } from './app.routes';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AuthService } from './services/auth';
import { ProfileService } from './services/DeliveryManDashboardService/profile-service';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MessageService } from 'primeng/api';
import { NgxSpinnerModule } from 'ngx-spinner';
export const appConfig: ApplicationConfig = {
  providers: [
     NgxSpinnerModule,
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes), provideClientHydration(withEventReplay()),
    provideHttpClient(withFetch()),
    provideHttpClient(withInterceptorsFromDi()),
    importProvidersFrom(FormsModule),
    AuthService,
    ProfileService,
    provideAnimationsAsync(),
    providePrimeNG({
      theme:{preset: Aura
      }
    }),
    importProvidersFrom(BrowserModule,BrowserAnimationsModule),
    MessageService
  ],

  
};
