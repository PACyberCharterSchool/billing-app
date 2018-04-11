import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import { User } from '../models/user.model';

@Injectable()
export class AuthenticationService {
  baseUrl = 'http://localhosts:5555/api/login';
  constructor(private http: HttpClient) { }

  authenticate(email: string, password: string): Observable<any> {
    const credentials = { email: email, password: password };
    return this.http.post<User>('/api/login', credentials);
  }
}
