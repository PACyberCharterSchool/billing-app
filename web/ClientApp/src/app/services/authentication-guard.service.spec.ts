import { TestBed, inject } from '@angular/core/testing';

import { RouterTestingModule } from '@angular/router/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { AuthenticationGuardService } from './authentication-guard.service';
import { AuthenticationService } from './authentication.service';

describe('AuthenticationGuardService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      providers: [
        AuthenticationGuardService,
        AuthenticationService,
        HttpClient,
        HttpHandler
      ]
    });
  });

  it('should be created', inject([AuthenticationGuardService], (service: AuthenticationGuardService) => {
    expect(service).toBeTruthy();
  }));
});
