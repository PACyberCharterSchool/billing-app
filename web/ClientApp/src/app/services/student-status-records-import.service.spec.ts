import { TestBed, inject } from '@angular/core/testing';

import { StudentStatusRecordsImportService } from './student-status-records-import.service';

describe('StudentStatusRecordsImportService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StudentStatusRecordsImportService]
    });
  });

  it('should be created', inject([StudentStatusRecordsImportService], (service: StudentStatusRecordsImportService) => {
    expect(service).toBeTruthy();
  }));
});
