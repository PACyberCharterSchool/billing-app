import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Student } from '../models/student.model';

@Injectable()
export class StudentsService {
  private apiStudentsUrl: string;

  constructor(private httpClient: HttpClient) {
    this.apiStudentsUrl = environment.apiBaseUrl + '/api/students';
  }

  public getStudents(): Observable<Student[]> {
    const headers = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };

    return this.httpClient.get<Student[]>(this.apiStudentsUrl, headers);
  }
}
