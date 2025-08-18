import { Injectable, signal } from '@angular/core';
import { ISideNavToggle } from '../../models/DeliveryManDashboardInterface/ISideNavToggle.interface';

@Injectable({ providedIn: 'root' })
export class SidenavService {
  private sideNavToggleSignal = signal<ISideNavToggle>({
    collapsed: false,
    screenWidth: 0,
  });

  setSideNavToggle(value: ISideNavToggle) {
    this.sideNavToggleSignal.set(value);
  }

  getSideNavToggle() {
    return this.sideNavToggleSignal.asReadonly();
  }
}
