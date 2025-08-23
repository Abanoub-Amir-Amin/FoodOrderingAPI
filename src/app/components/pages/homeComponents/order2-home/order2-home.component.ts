import {
  Component,
  Inject,
  PLATFORM_ID,
  AfterViewInit,
  OnDestroy,
} from '@angular/core';
import { isPlatformBrowser, ViewportScroller } from '@angular/common';
@Component({
  selector: 'app-order2-home',
  imports: [],
  templateUrl: './order2-home.component.html',
  styleUrl: './order2-home.component.css',
})
export class Order2HomeComponent implements AfterViewInit, OnDestroy {
  showScroll = false;
  private isBrowser = false;
  private onScrollHandler = () => this.checkScroll();

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private viewportScroller: ViewportScroller
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngAfterViewInit(): void {
    if (this.isBrowser) {
      window.addEventListener('scroll', this.onScrollHandler);
    }
  }

  ngOnDestroy(): void {
    if (this.isBrowser) {
      window.removeEventListener('scroll', this.onScrollHandler);
    }
  }

  private checkScroll(): void {
    const [x, y] = this.viewportScroller.getScrollPosition();
    this.showScroll = y >= 350;
  }

  scrollToTop(event: Event): void {
    event.preventDefault();
    if (this.isBrowser) {
      this.viewportScroller.scrollToPosition([0, 0]);
    }
  }
}
