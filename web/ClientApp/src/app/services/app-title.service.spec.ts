import { TestBed, inject } from '@angular/core/testing';

import { AppTitleService } from './app-title.service';

describe('AppTitleService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AppTitleService]
    });
  });

  it('should be created', inject([AppTitleService], (service: AppTitleService) => {
    expect(service).toBeTruthy();
  }));
});
