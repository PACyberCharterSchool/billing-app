import { TestBed, inject } from '@angular/core/testing';

import { SchoolDistrictService } from './school-district.service';

describe('SchoolDistrictService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SchoolDistrictService]
    });
  });

  it('should be created', inject([SchoolDistrictService], (service: SchoolDistrictService) => {
    expect(service).toBeTruthy();
  }));
});
