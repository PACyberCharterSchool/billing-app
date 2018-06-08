import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Report } from '../models/report.model';

import { Globals } from '../globals';

@Injectable()
export class ReportsService {
  private readonly apiReportsUrl = '/api/Reports';
  private readonly headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(
    private globals: Globals,
    private httpClient: HttpClient
  ) { }

  public getReports(skip: number): Observable<Report[]> {
    const url = this.apiReportsUrl;
    return this.httpClient.get<Report[]>(url, this.headers);
  }

  public getReport(id: number): Observable<Report> {
    const url = this.apiReportsUrl + `${id}`;
    return this.httpClient.get<Report>(url, this.headers);
  }

  public createReports(reportInfo: Report): Observable<Report[]> {
    const url = this.apiReportsUrl;
    return this.httpClient.post<Report[]>(url, reportInfo, this.headers);
  }
}
