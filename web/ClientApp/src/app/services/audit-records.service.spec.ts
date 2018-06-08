import { TestBed, inject } from '@angular/core/testing';

import { AuditRecordsService } from './audit-records.service';

describe('AuditRecordsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuditRecordsService]
    });
  });

  it('should be created', inject([AuditRecordsService], (service: AuditRecordsService) => {
    expect(service).toBeTruthy();
  }));
});
