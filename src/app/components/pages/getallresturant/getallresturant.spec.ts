import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Getallresturant } from './getallresturant';

describe('Getallresturant', () => {
  let component: Getallresturant;
  let fixture: ComponentFixture<Getallresturant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Getallresturant]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Getallresturant);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
