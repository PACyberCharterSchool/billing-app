import { TestBed, inject } from '@angular/core/testing';

import { AcademicYearsService } from './academic-years.service';

describe('AcademicYearsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AcademicYearsService]
    });
  });

  it('should be created', inject([AcademicYearsService], (service: AcademicYearsService) => {
    expect(service).toBeTruthy();
  }));
});
