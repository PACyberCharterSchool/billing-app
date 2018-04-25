import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Student } from '../models/student.model';

@Injectable()
export class StudentsService {
  private apiStudentsUrl: string;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private httpClient: HttpClient) {
    this.apiStudentsUrl = 'http://localhost:5000/api/students';
  }

  public getStudents(): Observable<Student[]> {
    return this.httpClient.get<Student[]>(this.apiStudentsUrl, this.headers);
  }

  public getStudent(id: number): Observable<Student> {
    return this.httpClient.get<Student>(this.apiStudentsUrl + `/${id}`, this.headers);
  }

  public getFilteredStudents(searchText: string): Observable<Student[]> {
    const url: string = this.buildStudentIdOrNameSearchQuery(searchText);
    return this.httpClient.get<Student[]>(url, this.headers);
  }

  private buildStudentIdOrNameSearchQuery(searchText: string): string {
    let url: string;
    let searchTokens: string[];
    let newSearchTokens: string[];

    if (searchText) {
      searchTokens = searchText.split(' ');
      newSearchTokens = searchTokens.map(
        (v) => {
          if (isNaN(Number(v))) {
            return `(FirstName eq ${v}) or (LastName eq ${v})`;
          } else {
            return `(PACyberId eq ${v}) or (PASecuredId eq ${v})`;
          }
        }
      );

      url = this.apiStudentsUrl + '?filter=' + '(' + newSearchTokens.join(' ') + ')';
      console.log('StudentsService.buildStudentIdOrNameSearchQuery(): url is ', url);
    }

    return url;
  }
}
