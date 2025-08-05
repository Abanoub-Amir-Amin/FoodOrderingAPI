import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResturanrtLogo } from './resturanrt-logo';

describe('ResturanrtLogo', () => {
  let component: ResturanrtLogo;
  let fixture: ComponentFixture<ResturanrtLogo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResturanrtLogo]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResturanrtLogo);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
