import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActionPending } from './action-pending';

describe('ActionPending', () => {
  let component: ActionPending;
  let fixture: ComponentFixture<ActionPending>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActionPending]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ActionPending);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
