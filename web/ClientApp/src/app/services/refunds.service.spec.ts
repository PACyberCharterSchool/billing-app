import { TestBed, inject } from '@angular/core/testing';

import { RefundsService } from './refunds.service';

describe('RefundsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RefundsService]
    });
  });

  it('should be created', inject([RefundsService], (service: RefundsService) => {
    expect(service).toBeTruthy();
  }));
});
