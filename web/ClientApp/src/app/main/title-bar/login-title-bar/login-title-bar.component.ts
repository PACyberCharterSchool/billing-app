import { Component, OnInit } from '@angular/core';
import { User } from '../../../models/user.model';

@Component({
  selector: 'app-login-title-bar',
  templateUrl: './login-title-bar.component.html',
  styleUrls: ['./login-title-bar.component.scss']
})
export class LoginTitleBarComponent implements OnInit {
  user: User = {
    id: 1,
    firstName: 'Fargus',
    lastName: 'McGinty'
  };

  constructor() { }

  ngOnInit() {
  }
}
