import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeliveryManRegister } from './delivery-man-register';

describe('DeliveryManRegister', () => {
  let component: DeliveryManRegister;
  let fixture: ComponentFixture<DeliveryManRegister>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeliveryManRegister]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeliveryManRegister);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
