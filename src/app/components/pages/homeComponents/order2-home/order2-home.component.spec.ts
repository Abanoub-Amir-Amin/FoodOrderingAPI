import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Order2HomeComponent } from './order2-home.component';

describe('Order2HomeComponent', () => {
  let component: Order2HomeComponent;
  let fixture: ComponentFixture<Order2HomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Order2HomeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Order2HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
