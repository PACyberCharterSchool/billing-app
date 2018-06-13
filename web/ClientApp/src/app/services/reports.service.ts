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
    data: '{ "DAY": [ { "FIELD2": "DATE", "FIELD3": "SCHOOL DAY", "FIELD4": "MEMBERSHIP" } ], "Wednesday": [ { "FIELD2": "8/30/2017", "FIELD3": "1", "FIELD4": "180" }, { "FIELD2": "9/6/2017", "FIELD3": "5", "FIELD4": "176" }, { "FIELD2": "9/13/2017", "FIELD3": "10", "FIELD4": "171" }, { "FIELD2": "9/20/2017", "FIELD3": "15", "FIELD4": "166" }, { "FIELD2": "9/27/2017", "FIELD3": "20", "FIELD4": "161" }, { "FIELD2": "10/4/2017", "FIELD3": "25", "FIELD4": "156" }] }',
    xlsx: null
  },
  {
    id: 2,
    type: ReportType.Invoice,
    schoolYear: '2017-2018',
    name: 'Seneca Valley SD',
    approved: false,
    created: new Date('2018-06-09'),
    data: '{ "DAY": { "FIELD2": "DATE", "FIELD3": "SCHOOL DAY", "FIELD4": "MEMBERSHIP" }, "Nonceday": { "FIELD2": "8/30/2017", "FIELD3": "1", "FIELD4": "180" } }',
    xlsx: null
  },
  {
    id: 3,
    type: ReportType.Invoice,
    schoolYear: '2017-2018',
    name: 'Montour Run SD',
    approved: true,
    created: new Date('2018-04-09'),
    data: '[{ "Day": "Day" }, { "Night": "Night" }]',
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
    // const url = this.apiReportsUrl + `/${type}`;
    // return this.httpClient.get<Report[]>(url, this.headers);
    return Observable.of(reportsMeta);
  }

  public getReport(id: number): Observable<Report> {
    const url = this.apiReportsUrl + `/${id}`;
    return this.httpClient.get<Report>(url, this.headers);
  }

  public getReportByName(name: string): Observable<Report> {
    const url = this.apiReportsUrl + `/${name}`;
    return this.httpClient.get<Report>(url, this.headers);
  }

  public createReports(reportInfo: Report): Observable<Report[]> {
    const url = this.apiReportsUrl + '/many';
    return this.httpClient.post<Report[]>(url, reportInfo, this.headers);
  }
}
