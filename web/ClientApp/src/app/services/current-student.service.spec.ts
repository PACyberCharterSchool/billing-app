import { TestBed, inject } from '@angular/core/testing';

import { CurrentStudentService } from './current-student.service';

describe('CurrentStudentService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CurrentStudentService]
    });
  });

  it('should be created', inject([CurrentStudentService], (service: CurrentStudentService) => {
    expect(service).toBeTruthy();
  }));
});
