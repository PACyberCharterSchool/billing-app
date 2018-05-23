import { TestBed, inject } from '@angular/core/testing';

import { SchoolCalendarService } from './school-calendar.service';

describe('SchoolCalendarService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SchoolCalendarService]
    });
  });

  it('should be created', inject([SchoolCalendarService], (service: SchoolCalendarService) => {
    expect(service).toBeTruthy();
  }));
});
