import { Component, OnInit } from '@angular/core';
import { User } from '../../../models/user.model';

@Component({
  selector: 'app-login-title-bar',
  templateUrl: './login-title-bar.component.html',
  styleUrls: ['./login-title-bar.component.scss']
})
export class LoginTitleBarComponent implements OnInit {
  user: User = new User();

  constructor() { }

  ngOnInit() {
    const userName = localStorage.getItem('current-user');
    let names: string[];

    names = userName.split(' ');
    this.user.firstName = names[0];
    this.user.lastName = names[1];
  }
}
