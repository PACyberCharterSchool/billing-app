import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { PendingStudentStatusRecord } from '../models/pending-student-status-record.model';

import { Globals } from '../globals';

import { environment } from '../../environments/environment';

@Injectable()
export class StudentStatusRecordsImportService {
  private apiSSRUrl;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private globals: Globals, private httpClient: HttpClient) {
    this.apiSSRUrl = 'http://' + environment.apiHostName + ':' + environment.apiHostPort + '/api/StudentStatusRecords';
  }

  public getPending(skip: number): Observable<PendingStudentStatusRecord[]> {
    const url = this.apiSSRUrl + `/pending?skip=${skip}&take=${this.globals.take}`;
    return this.httpClient.get<PendingStudentStatusRecord[]>(url, this.headers);
  }

  public postStudentData(): Observable<any> {
    const url = this.apiSSRUrl + `/pending/commit`;
    return this.httpClient.post<any>(url, this.headers);
  }
}
