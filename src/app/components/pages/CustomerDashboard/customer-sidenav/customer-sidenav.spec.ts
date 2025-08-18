import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerSidenav } from './customer-sidenav';

describe('CustomerSidenav', () => {
  let component: CustomerSidenav;
  let fixture: ComponentFixture<CustomerSidenav>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerSidenav]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerSidenav);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
