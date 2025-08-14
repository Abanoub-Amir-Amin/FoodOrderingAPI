import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountHomeComponent } from './count-home.component';

describe('CountHomeComponent', () => {
  let component: CountHomeComponent;
  let fixture: ComponentFixture<CountHomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CountHomeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CountHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
