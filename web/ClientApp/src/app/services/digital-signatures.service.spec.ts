import { TestBed, inject } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { DigitalSignaturesService } from './digital-signatures.service';

import { Globals } from '../globals';

describe('DigitalSignaturesService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DigitalSignaturesService, Globals, HttpClient, HttpHandler]
    });
  });

  it('should be created', inject([DigitalSignaturesService], (service: DigitalSignaturesService) => {
    expect(service).toBeTruthy();
  }));
});
