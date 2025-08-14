import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RestaurantApply } from './restaurant-apply';

describe('RestaurantApply', () => {
  let component: RestaurantApply;
  let fixture: ComponentFixture<RestaurantApply>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RestaurantApply]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RestaurantApply);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
