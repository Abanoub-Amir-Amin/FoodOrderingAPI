import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PopularRestaurantsHomeComponent } from './popular-restaurants-home.component';

describe('PopularRestaurantsHomeComponent', () => {
  let component: PopularRestaurantsHomeComponent;
  let fixture: ComponentFixture<PopularRestaurantsHomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PopularRestaurantsHomeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PopularRestaurantsHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
