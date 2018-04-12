import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { User } from '../models/user.model';

import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable()
export class AuthenticationService {
  baseUrl = 'http://localhost:5000/api/auth/login';
  constructor(private http: HttpClient) { }

  public isAuthenticated(): boolean {
    const token = localStorage.getItem('jwt-token');
    const jwtHelper = new JwtHelperService();

    console.log('AuthenticationService.isAuthenticated():  token is ', token);

    // returns true or false based upon whether the JWT is empty or expired...
    return ((token !== null) || !jwtHelper.isTokenExpired(token));
  }

  public authenticate(email: string, password: string): Observable<any> {
    const headers = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };

    const credentials = { username: email, password: password };
    return this.http.post(this.baseUrl, JSON.stringify(credentials), headers);
  }
}
