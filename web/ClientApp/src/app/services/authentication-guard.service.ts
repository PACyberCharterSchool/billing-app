import { Injectable } from '@angular/core';

import { Router, CanActivate } from '@angular/router';

import { AuthenticationService } from '../services/authentication.service';

@Injectable()
export class AuthenticationGuardService {

  constructor(public auth: AuthenticationService, public router: Router) { }

  canActivate(): boolean {
    console.log(`AuthenticationGuardService.canActivate():  auth is #{this.auth}.`);
    if (!this.auth.isAuthenticated()) {
      this.router.navigate(['login']);
      return false;
    }

    return true;
  }

}
