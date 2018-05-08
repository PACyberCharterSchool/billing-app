import { TestBed, inject } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { StudentStatusRecordsImportService } from './student-status-records-import.service';

import { Globals } from '../globals';

describe('StudentStatusRecordsImportService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StudentStatusRecordsImportService, Globals, HttpClient, HttpHandler ]
    });
  });

  it('should be created', inject([StudentStatusRecordsImportService], (service: StudentStatusRecordsImportService) => {
    expect(service).toBeTruthy();
  }));
});
