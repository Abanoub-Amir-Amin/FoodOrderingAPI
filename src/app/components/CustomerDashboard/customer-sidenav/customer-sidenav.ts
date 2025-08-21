import { isPlatformBrowser, NgClass, NgIf } from '@angular/common';
import { Component, ElementRef, HostListener, Inject, OnInit, PLATFORM_ID, ViewChild } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SidenavService } from '../../../services/DeliveryManDashboardService/sidenav.service';
import { AuthService } from '../../../services/auth';
import { customernavbarData } from './customer-navbarData';
@Component({
  selector: 'app-customer-sidenav',
  imports: [RouterLink,RouterLinkActive,NgClass,NgIf],
  templateUrl: './customer-sidenav.html',
  styleUrl: './customer-sidenav.css'
})
export class CustomerSidenav implements OnInit {
   collapsed = false;
  screenWith = 0;
  navData = customernavbarData;
  @ViewChild('sidenav', { static: false }) SideNav!: ElementRef;

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
@HostListener('document:click', ['$event'])
  handleClickOutside(event: Event) {
    const clickedInside = this.SideNav.nativeElement.contains(event.target);
    
    if (!clickedInside ) {
     this.closeSidenav()
    }
  }
}
