import { Component, OnInit, ViewChild } from '@angular/core';

import { Router } from '@angular/router';

import { AuthenticationService } from '../../../services/authentication.service';

@Component({
  selector: 'app-login-popup',
  templateUrl: './login-popup.component.html',
  styleUrls: ['./login-popup.component.scss']
})
export class LoginPopupComponent implements OnInit {

  constructor(private router: Router, private auth: AuthenticationService) { }

  ngOnInit() {
  }

  onLogoutClick() {
    console.log('LoginPopupComponent.onLogoutClick(): calling logout().');
    this.logout();
  }

  private logout() {
    if (this.auth.isAuthenticated()) {
      console.log('LoginPopupComponent.logout():  user is authenticated.  Removing JWT token.');
      localStorage.removeItem('jwt-token');
    }

    this.router.navigate(['login']);
  }
}
