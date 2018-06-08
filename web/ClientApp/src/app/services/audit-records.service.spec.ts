import { TestBed, inject } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { AuditRecordsService } from './audit-records.service';
import { Globals } from '../globals';

describe('AuditRecordsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuditRecordsService, Globals, HttpClient, HttpHandler]
    });
  });

  it('should be created', inject([AuditRecordsService], (service: AuditRecordsService) => {
    expect(service).toBeTruthy();
  }));
});
