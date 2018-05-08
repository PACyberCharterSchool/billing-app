import { TestBed, inject } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { StudentsService } from './students.service';

import { Globals } from '../globals';

describe('StudentsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StudentsService, HttpClient, HttpHandler, Globals]
    });
  });

  it('should be created', inject([StudentsService], (service: StudentsService) => {
    expect(service).toBeTruthy();
  }));
});
