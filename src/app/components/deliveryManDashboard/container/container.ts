import { Component, inject, effect, computed } from '@angular/core';
import { Sidenav } from '../sidenav/sidenav';
import { Body } from '../body/body';
import { SidenavService } from '../../../services/DeliveryManDashboardService/sidenav.service';
import { Header } from '../header/header';

@Component({
  selector: 'app-container',
  imports: [Sidenav, Body, Header],
  templateUrl: './container.html',
  styleUrl: './container.css',
  standalone: true,
})
export class Container {
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
