import { TestBed, inject } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { TemplatesService } from './templates.service';

import { Globals } from '../globals';

describe('TemplatesService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TemplatesService,
        Globals,
        HttpClient,
        HttpHandler
      ]
    });
  });

  it('should be created', inject([TemplatesService], (service: TemplatesService) => {
    expect(service).toBeTruthy();
  }));
});
