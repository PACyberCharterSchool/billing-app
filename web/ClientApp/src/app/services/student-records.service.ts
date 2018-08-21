import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';

import { Globals } from '../globals';

import { environment } from '../../environments/environment';

import { StudentRecordsHeader, StudentRecord } from '../models/student-record.model';

@Injectable()
export class StudentRecordsService {
  private apiSSRUrl;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private globals: Globals, private httpClient: HttpClient) {
    this.apiSSRUrl = 'api/StudentRecords';
  }

  public postLockStudentData(scope: string): Observable<any> {
    const url = this.apiSSRUrl + `/header/${scope}/lock`;
    return this.httpClient.post<any>(url, this.headers);
  }

  public getStudentRecordsHeaders(): Observable<StudentRecordsHeader[]> {
    const url = this.apiSSRUrl + `/scopes`;
    return this.httpClient.get<StudentRecordsHeader[]>(url, this.headers);
    // return Observable.of(studentRecordsHeaders);
  }

  public getStudentRecordsHeaderByScope(scope: string, skip: number): Observable<StudentRecordsHeader> {
    const url = this.apiSSRUrl + `/header/${scope}?Skip=${skip}&Take=${this.globals.take}`;
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }

  public updateStudentRecord(scope: string, record: StudentRecord): Observable<StudentRecord> {
    const url = this.apiSSRUrl + `/${scope}/${record.id}`;

    delete record.id;
    delete record.header;
    delete record.lazyLoader;
    delete record.lastUpdated;

    return this.httpClient.put<StudentRecord>(url, record, this.headers);
  }
}
