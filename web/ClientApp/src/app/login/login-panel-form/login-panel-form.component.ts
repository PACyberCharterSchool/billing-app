import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import * as HttpStatus from 'http-status-codes';

import { AuthenticationService } from '../../services/authentication.service';

import { User } from '../../models/user.model';

@Component({
  selector: 'app-login-panel-form',
  templateUrl: './login-panel-form.component.html',
  styleUrls: ['./login-panel-form.component.scss']
})
export class LoginPanelFormComponent implements OnInit {

  public email: string;
  public password: string;
  public loginError: string;

  constructor(private router: Router, private authService: AuthenticationService) { }

  ngOnInit() {
  }

  private handleLoginError(error: HttpErrorResponse) {
    console.log('LoginPanelFormComponent.handleLoginError(): error status is ', error.status);

    switch (error.status) {
      case HttpStatus.UNAUTHORIZED: {
        this.loginError = 'The username or password were incorrect.';
        break;
      }
      case HttpStatus.FORBIDDEN: {
        this.loginError = 'The username has been forbidden access to the application.';
        break;
      }
      case HttpStatus.BAD_REQUEST: {
        this.loginError = 'The username or password were not in the correct format.';
        break;
      }
      default: {
        this.loginError = null;
        break;
      }
    }
  }

  login(): void {
    console.log('LoginPanelFormComponent.login():  email is ', this.email);
    console.log('LoginPanelFormComponent.login():  password is ', this.password);

    let user: User = new User();

    this.authService.authenticate(this.email, this.password).subscribe(
      data => {
        console.log('LoginPanelFormComponent.login():  authentication successful.  data is ', data);
        localStorage.setItem('jwt-token', data.token);
        user.firstName = data.token;
        localStorage.setItem('current-user', user.firstName);
        this.router.navigate(['main']);
      },
      error => {
        this.handleLoginError(error);
        console.log('LoginPanelFormComponent.login():  authentication failed.  error is ', error);
      }
    );
  }
}
