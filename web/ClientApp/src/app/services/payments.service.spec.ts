import { TestBed, inject } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { PaymentsService } from './payments.service';

import { Globals } from '../globals';

describe('PaymentsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ PaymentsService, Globals, HttpClient, HttpHandler ]
    });
  });

  it('should be created', inject([PaymentsService], (service: PaymentsService) => {
    expect(service).toBeTruthy();
  }));
});
