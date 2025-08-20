import { Component, HostListener, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../../services/auth';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, NgIf],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  menuOpen = false; // State to manage the menu visibility
  constructor(public authService: AuthService, private router: Router) {}

  login() {
    // Implement login logic or redirect to login page
    this.router.navigate(['/login']);
  }

  signup() {
    this.router.navigate(['/customerRegister']);
  }

  logout() {
    this.authService.logout();
  }

  isOpen = signal(false);

  toggleMenu() {
    this.isOpen.update((v) => !v);
  }
  closeMenu() {
    this.isOpen.set(false);
  }

  @HostListener('window:resize')
  onResize() {
    if (window.innerWidth > 820 && this.isOpen()) this.isOpen.set(false);
  }
}
