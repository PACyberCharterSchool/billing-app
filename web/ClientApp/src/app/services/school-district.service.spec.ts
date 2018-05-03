import { TestBed, inject } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { SchoolDistrictService } from './school-district.service';

describe('SchoolDistrictService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SchoolDistrictService, HttpClient, HttpHandler]
    });
  });

  it('should be created', inject([SchoolDistrictService], (service: SchoolDistrictService) => {
    expect(service).toBeTruthy();
  }));
});
