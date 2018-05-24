import { TestBed, inject } from '@angular/core/testing';
import { HttpClient, HttpHandler } from '@angular/common/http';

import { SchoolCalendarService } from './school-calendar.service';

describe('SchoolCalendarService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SchoolCalendarService, HttpClient, HttpHandler]
    });
  });

  it('should be created', inject([SchoolCalendarService], (service: SchoolCalendarService) => {
    expect(service).toBeTruthy();
  }));
});
