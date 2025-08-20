import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerInbobox } from './customer-inbobox';

describe('CustomerInbobox', () => {
  let component: CustomerInbobox;
  let fixture: ComponentFixture<CustomerInbobox>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerInbobox]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerInbobox);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
