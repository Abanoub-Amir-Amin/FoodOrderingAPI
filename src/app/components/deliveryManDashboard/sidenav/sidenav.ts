import {
  Component,
  HostListener,
  OnInit,
  Inject,
  PLATFORM_ID,
} from '@angular/core';
import { navbarData } from './nav-data';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NgClass, NgIf, isPlatformBrowser } from '@angular/common';
import { SidenavService } from '../../../services/DeliveryManDashboardService/sidenav.service';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-sidenav',
  standalone: true,
  imports: [RouterLink, NgIf, NgClass, RouterLinkActive],
  templateUrl: './sidenav.html',
  styleUrl: './sidenav.css',
})
export class Sidenav implements OnInit {
  collapsed = false;
  screenWith = 0;
  navData = navbarData;

  constructor(
    private sidenavService: SidenavService,
    @Inject(PLATFORM_ID) private platformId: Object,
    private loginService: AuthService
  ) {}
  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.screenWith = window.innerWidth;
    }
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    if (typeof window !== 'undefined') {
      this.screenWith = window.innerWidth;
      if (this.screenWith <= 768) {
        this.collapsed = false;
        this.sidenavService.setSideNavToggle({
          collapsed: this.collapsed,
          screenWidth: this.screenWith,
        });
      }
    }
  }
  toggleCollapse(): void {
    this.collapsed = !this.collapsed;
    this.sidenavService.setSideNavToggle({
      collapsed: this.collapsed,
      screenWidth: this.screenWith,
    });
  }

  closeSidenav(): void {
    this.collapsed = false;
    this.sidenavService.setSideNavToggle({
      collapsed: this.collapsed,
      screenWidth: this.screenWith,
    });
  }

  loggedOut(event: Event): void {
    event.preventDefault();
    this.loginService.logout();
  }
}
