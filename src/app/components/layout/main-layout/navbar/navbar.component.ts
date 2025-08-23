import {
  Component,
  HostListener,
  signal,
  ElementRef,
  Renderer2,
  ViewChild,
} from '@angular/core';
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
  menuOpen = false;

  constructor(
    public authService: AuthService,
    private router: Router,
    private renderer: Renderer2
  ) {}

  // login() {
  //   this.router.navigate(['/login']);
  // }

  signup() {
    this.router.navigate(['/customerRegister']);
  }

  logout() {
    this.authService.logout();
  }

  // الطريقة الجديدة للـ scroll مع إغلاق الـ dropdown
  scrollTo(section: 'restaurant' | 'aboutUs' | 'contactUs' | 'joinUs'): void {
    let targetId = '';

    switch (section) {
      case 'restaurant':
        targetId = 'restaurant';
        break;
      case 'aboutUs':
        targetId = 'aboutUs';
        break;
      case 'contactUs':
        targetId = 'Contact';
        break;
      case 'joinUs':
        targetId = 'joinUs';
        break;
    }

    // إغلاق الـ dropdown فوراً
    this.forceCloseDropdown();

    const element = document.getElementById(targetId);
    if (element) {
      // انتظار شوية عشان الـ animation يخلص
      setTimeout(() => {
        element.scrollIntoView({
          behavior: 'smooth',
          block: 'start',
        });
      }, 200);
    } else {
      console.warn(`Element with ID '${targetId}' not found`);
    }
  }

  // طريقة جديدة لإغلاق الـ dropdown بقوة
  forceCloseDropdown(): void {
    if (this.dropDownMenu && this.toggleBtnIcon) {
      const dropDown = this.dropDownMenu.nativeElement as HTMLElement;
      const icon = this.toggleBtnIcon.nativeElement as HTMLElement;

      // إزالة الـ open class فوراً
      dropDown.classList.remove('open');
      this.menuOpen = false;

      // تغيير الأيقونة للشكل العادي فوراً
      icon.className = 'fa-solid fa-bars-staggered';
    }
  }

  // طريقة عادية لإغلاق الـ dropdown
  closeDropdown(): void {
    this.forceCloseDropdown();
  }

  // للتعامل مع الـ login وإغلاق الـ dropdown
  login() {
    this.forceCloseDropdown();
    this.router.navigate(['/login']);
  }

  // للتعامل مع الـ Home scroll
  scrollToHome(): void {
    this.forceCloseDropdown();

    const element = document.getElementById('home');
    if (element) {
      setTimeout(() => {
        element.scrollIntoView({
          behavior: 'smooth',
          block: 'start',
        });
      }, 200);
    } else {
      // لو مش لاقي الـ home element، اعمل scroll لأعلى الصفحة
      setTimeout(() => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }, 200);
    }
  }

  // للتعامل مع الـ Dashboard navigation
  navigateToDashboard(): void {
    this.forceCloseDropdown();
    this.router.navigate(['/CustomerDashboard']);
  }

  //Dropdown Menu
  @ViewChild('toggleBtnIcon') toggleBtnIcon!: ElementRef;
  @ViewChild('dropDownMenu') dropDownMenu!: ElementRef;

  onToggleMenu(): void {
    const dropDown = this.dropDownMenu.nativeElement as HTMLElement;
    const icon = this.toggleBtnIcon.nativeElement as HTMLElement;

    dropDown.classList.toggle('open');
    this.menuOpen = !this.menuOpen;

    const isOpen = dropDown.classList.contains('open');
    this.renderer.setAttribute(
      icon,
      'class',
      isOpen ? 'fa-solid fa-xmark' : 'fa-solid fa-bars-staggered'
    );
  }

  // إغلاق الـ dropdown لو المستخدم دوس في أي مكان تاني
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    const target = event.target as HTMLElement;
    const navbar = target.closest('.navbar');

    // لو الـ click مش جوا الـ navbar، اقفل الـ dropdown
    if (!navbar && this.menuOpen) {
      this.closeDropdown();
    }
  }

  // للتأكد إن الـ dropdown يقفل لو المستخدم عمل scroll
  @HostListener('window:scroll')
  onWindowScroll(): void {
    if (this.menuOpen) {
      this.closeDropdown();
    }
  }

  GoToCart(): void {
    this.router.navigate(['/shoppingcart']);
  }
}
