import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Newpassword } from './newpassword';

describe('Newpassword', () => {
  let component: Newpassword;
  let fixture: ComponentFixture<Newpassword>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Newpassword]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Newpassword);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
