import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MostOrdered } from './most-ordered';

describe('MostOrdered', () => {
  let component: MostOrdered;
  let fixture: ComponentFixture<MostOrdered>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MostOrdered]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MostOrdered);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
