import { TestBed, inject } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { StudentRecordsImportService } from './student-records-import.service';

import { Globals } from '../globals';

describe('StudentRecordsImportService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StudentRecordsImportService, Globals, HttpClient, HttpHandler ]
    });
  });

  it('should be created', inject([StudentRecordsImportService], (service: StudentRecordsImportService) => {
    expect(service).toBeTruthy();
  }));
});
