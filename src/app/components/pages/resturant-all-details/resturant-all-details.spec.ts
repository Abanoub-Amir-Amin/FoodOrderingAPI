import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResturantAllDetails } from './resturant-all-details';

describe('ResturantAllDetails', () => {
  let component: ResturantAllDetails;
  let fixture: ComponentFixture<ResturantAllDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResturantAllDetails]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResturantAllDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
