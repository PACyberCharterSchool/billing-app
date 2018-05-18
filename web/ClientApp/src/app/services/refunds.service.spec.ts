import { TestBed, inject } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { RefundsService } from './refunds.service';

import { Globals } from '../globals';

describe('RefundsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ RefundsService, Globals, HttpClient, HttpHandler ]
    });
  });

  it('should be created', inject([RefundsService], (service: RefundsService) => {
    expect(service).toBeTruthy();
  }));
});
