import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerContainer } from './customer-container';

describe('CustomerContainer', () => {
  let component: CustomerContainer;
  let fixture: ComponentFixture<CustomerContainer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerContainer]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerContainer);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
