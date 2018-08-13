import { TestBed, inject } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { StudentRecordsService } from './student-records.service';

import { Globals } from '../globals';

describe('StudentRecordsImportService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StudentRecordsService, Globals, HttpClient, HttpHandler ]
    });
  });

  it('should be created', inject([StudentRecordsService], (service: StudentRecordsService) => {
    expect(service).toBeTruthy();
  }));
});
