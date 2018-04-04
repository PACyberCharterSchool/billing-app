import { Component, OnInit } from '@angular/core';
import { User } from '../models/user';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  user: User = {
    id: 1,
    firstName: 'Fargus',
    lastName: 'McGinty'
  };

  constructor() { }

  ngOnInit() {
  }
}
