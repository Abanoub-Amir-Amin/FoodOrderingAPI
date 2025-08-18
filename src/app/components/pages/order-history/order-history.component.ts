import { Component } from '@angular/core';

import { HeaderComponent } from '../../layout/main-layout/header/header.component';

import { FirstcomponentComponent } from '../order/firstcomponent/firstcomponent.component';
import { LocationComponent } from '../order/location/location.component';
import { NavbarComponent } from '../../layout/main-layout/navbar/navbar.component';
import { Footer } from '../../../components/layout/footer/footer';

@Component({
  selector: 'app-order-history',
  imports: [
    HeaderComponent,
    NavbarComponent,
    FirstcomponentComponent,
    LocationComponent,
    Footer,
  ],
  templateUrl: './order-history.component.html',
  styleUrl: './order-history.component.css',
})
export class OrderHistoryComponent {}
