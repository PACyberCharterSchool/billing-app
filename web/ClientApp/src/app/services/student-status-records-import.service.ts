import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { PendingStudentStatusRecord } from '../models/pending-student-status-record.model';

import { Globals } from '../globals';

@Injectable()
export class StudentStatusRecordsImportService {
  private apiSSRUrl = 'http://localhost:5000/api/StudentStatusRecords';
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private globals: Globals, private httpClient: HttpClient) {
  }

  public getPending(skip: number): Observable<PendingStudentStatusRecord[]> {
    const url = this.apiSSRUrl + `?skip=${skip}&take=${this.globals.take}`;
    return this.httpClient.get<PendingStudentStatusRecord[]>(this.apiSSRUrl + '/pending?skip=0&take=100', this.headers);
  }
}
