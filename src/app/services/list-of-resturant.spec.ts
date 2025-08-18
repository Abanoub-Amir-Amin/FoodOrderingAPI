import { TestBed } from '@angular/core/testing';

import { ListOfResturant } from './list-of-resturant';

describe('ListOfResturant', () => {
  let service: ListOfResturant;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ListOfResturant);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
