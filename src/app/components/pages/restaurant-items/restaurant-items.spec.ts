import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RestaurantItems } from './restaurant-items';

describe('RestaurantItems', () => {
  let component: RestaurantItems;
  let fixture: ComponentFixture<RestaurantItems>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RestaurantItems]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RestaurantItems);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
