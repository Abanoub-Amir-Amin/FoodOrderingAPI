import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerAddresses } from './customer-addresses';

describe('CustomerAddresses', () => {
  let component: CustomerAddresses;
  let fixture: ComponentFixture<CustomerAddresses>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerAddresses]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerAddresses);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
