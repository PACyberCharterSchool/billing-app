import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

// import { Observable } from 'rxjs/Observable';
import { Observable } from 'rxjs/Rx';

import { environment } from '../../environments/environment';

import { Report, ReportType } from '../models/report.model';

import { Globals } from '../globals';

const reportsMeta: Report[] = [
  {
    id: 1,
    type: ReportType.Invoice,
    schoolYear: '2017-2018',
    name: 'Mars School SD',
    approved: true,
    created: new Date('2018-05-09'),
    data: null,
    xlsx: null
  },
  {
    id: 2,
    type: ReportType.Invoice,
    schoolYear: '2017-2018',
    name: 'Seneca Valley SD',
    approved: false,
    created: new Date('2018-06-09'),
    data: null,
    xlsx: null
  },
  {
    id: 3,
    type: ReportType.Invoice,
    schoolYear: '2017-2018',
    name: 'Montour Run SD',
    approved: true,
    created: new Date('2018-04-09'),
    data: null,
    xlsx: null
  }
];

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

  public getReportsByType(type: string, skip: number): Observable<Report[]> {
    // const url = this.apiReportsUrl + `${type}`;
    // return this.httpClient.get<Report[]>(url, this.headers);
    return Observable.of(reportsMeta);
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
