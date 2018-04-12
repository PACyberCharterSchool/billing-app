import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-login-panel-form',
  templateUrl: './login-panel-form.component.html',
  styleUrls: ['./login-panel-form.component.scss']
})
export class LoginPanelFormComponent implements OnInit {

  private email: string;
  private password: string;

  constructor(private router: Router, private authService: AuthenticationService) { }

  ngOnInit() {
  }

  login(): void {
    console.log('LoginPanelFormComponent.login():  email is ', this.email);
    console.log('LoginPanelFormComponent.login():  password is ', this.password);

    this.authService.authenticate(this.email, this.password).subscribe(
      data => {
        console.log('LoginPanelFormComponent.login():  authentication successful.  data is ', data);
        localStorage.setItem('jwt-token', data.token);
        this.router.navigate(['']);
      },
      error => {
        console.log('LoginPanelFormComponent.login():  authentication failed.  error is ', error);
      }
    );
  }
}
