import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RestaurantDashboard } from './dashboard';

describe('Dashboard', () => {
  let component: RestaurantDashboard;
  let fixture: ComponentFixture<RestaurantDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RestaurantDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RestaurantDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
