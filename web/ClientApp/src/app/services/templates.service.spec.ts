import { TestBed, inject } from '@angular/core/testing';

import { TemplatesService } from './templates.service';

describe('TemplatesService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TemplatesService]
    });
  });

  it('should be created', inject([TemplatesService], (service: TemplatesService) => {
    expect(service).toBeTruthy();
  }));
});
