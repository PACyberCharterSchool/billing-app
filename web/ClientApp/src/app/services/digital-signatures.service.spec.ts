import { TestBed, inject } from '@angular/core/testing';

import { DigitalSignaturesService } from './digital-signatures.service';

describe('DigitalSignaturesService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DigitalSignaturesService]
    });
  });

  it('should be created', inject([DigitalSignaturesService], (service: DigitalSignaturesService) => {
    expect(service).toBeTruthy();
  }));
});
