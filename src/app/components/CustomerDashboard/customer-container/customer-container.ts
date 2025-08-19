import { Component, effect, inject } from '@angular/core';
import { CustomerSidenav } from '../customer-sidenav/customer-sidenav';
import { CustomerBody } from '../customer-body/customer-body';
import { CustomerHeader } from '../customer-header/customer-header';
import { SidenavService } from '../../../services/DeliveryManDashboardService/sidenav.service';
import { MainLayoutComponent } from '../../layout/main-layout/main-layout.component';

@Component({
  selector: 'app-customer-container',
  imports: [CustomerSidenav,CustomerBody,CustomerHeader],
  templateUrl: './customer-container.html',
  styleUrl: './customer-container.css'
})
export class CustomerContainer {
title = 'sidenav';
  private sidenavService = inject(SidenavService);

  sideNavState = this.sidenavService.getSideNavToggle();

  isSideNavCollapsed = false;
  screenWidth = 0;

  constructor() {
    effect(() => {
      const { collapsed, screenWidth } = this.sideNavState();
      this.isSideNavCollapsed = collapsed;
      this.screenWidth = screenWidth;
    });
  }
}
