import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { Globals } from '../globals';

import { environment } from '../../environments/environment';

import { StudentRecordsHeader, StudentRecord } from '../models/student-record.model';

@Injectable()
export class StudentRecordsImportService {
  private apiSSRUrl;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private globals: Globals, private httpClient: HttpClient) {
    this.apiSSRUrl = 'api/StudentRecords';
  }

  public postStudentData(): Observable<any> {
    const url = this.apiSSRUrl + `/pending/commit`;
    return this.httpClient.post<any>(url, this.headers);
  }

  public getStudentRecordsHeaders(): Observable<StudentRecordsHeader[]> {
    const url = this.apiSSRUrl + `/headers`;
    return this.httpClient.get<StudentRecordsHeader[]>(url, this.headers);
  }

  public getStudentRecordsHeaderByScope(scope: string): Observable<StudentRecordsHeader> {
    const url = this.apiSSRUrl + `/header/${scope}`;
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }
}
