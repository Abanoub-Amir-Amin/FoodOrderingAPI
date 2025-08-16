import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerBody } from './customer-body';

describe('CustomerBody', () => {
  let component: CustomerBody;
  let fixture: ComponentFixture<CustomerBody>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerBody]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerBody);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
