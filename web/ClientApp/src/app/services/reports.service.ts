import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { Report, ReportType } from '../models/report.model';

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

  // HTTP GET /api/reports
  public getReportsByMeta(reportInfo: Object): Observable<Report[]> {
    let url = this.apiReportsUrl;

    if (reportInfo['Type']) { url += `?Type=${reportInfo['Type']}`; } // there will *always* be a Type
    if (reportInfo['Name']) { url += `&Name=${reportInfo['Name']}`; }
    if (reportInfo['SchoolYear']) { url += `&SchoolYear=${reportInfo['SchoolYear']}`; }
    if (reportInfo['Approved']) { url += `&Approved=${reportInfo['Approved']}`; }
    if (reportInfo['Scope']) { url += `&Scope=${reportInfo['Scope']}`; }

    return this.httpClient.get<Report[]>(url, this.headers);
  }

  public getAccountsReceivableAsOf(): Observable<Report[]> {
    const reportMeta: Object = Object.assign({}, {
      'Type': ReportType.AccountsReceivableAsOf,
    });

    return this.getReportsByMeta(reportMeta);
  }

  public getAccountsReceivableAging(): Observable<Report[]> {
    const reportMeta: Object = Object.assign({}, {
      'Type': ReportType.AccountsReceivableAging,
    });

    return this.getReportsByMeta(reportMeta);
  }

  public getCSIU(): Observable<Report[]> {
    const reportMeta: Object = Object.assign({}, {
      'Type': ReportType.CSIU,
    });

    return this.getReportsByMeta(reportMeta);
  }

  // HTTP GET /api/reports/:name
  public getReportDataByFormat(report: Report, format: string): Observable<any> {
    const url = this.apiReportsUrl + `/${report.name}`;
    const accept: string = format === 'excel' ? 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' : 'application/pdf';
    const headers = {};
    headers['responseType'] = 'arrayBuffer';
    headers['headers'] = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': accept
    });

    return this.httpClient.get<any>(url, headers);
  }

  public getInvoices(name: string, year: string, scope: string, approved: boolean): Observable<Report[]> {
    const reportInfo: Object = Object.assign({}, { 'Type': ReportType.Invoice });

    if (name) { reportInfo['Name'] = name; }
    if (year) { reportInfo['SchoolYear'] = year; }
    if (approved != null) { reportInfo['Approved'] = approved; }
    if (scope) { reportInfo['Scope'] = scope; }

    return this.getReportsByMeta(reportInfo);
  }

  public getBulkInvoices(year: string, scope: string): Observable<any> {
    const reportInfo: Object = Object.assign({}, { 'Type': ReportType.BulkInvoice });

    if (year) { reportInfo['SchoolYear'] = year; }
    if (scope) { reportInfo['Scope'] = scope; }

    return this.getBulkInvoiceByInfo(reportInfo);
  }

  public getInvoiceByName(name: string): Observable<any> {
    const headers = {};
    headers['responseType'] = 'arrayBuffer';
    headers['headers'] = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/pdf'
    });

    const url = this.apiReportsUrl + `/${name}`;
    return this.httpClient.get<any>(url, headers);
  }

  public getInvoicesBySchoolYearAndStatus(year: string, status: string): Observable<Report[]> {
    const url = this.apiReportsUrl + `?year=${year}&status=${status}`;
    return this.httpClient.get<Report[]>(url, this.headers);
  }

  public getInvoiceStudentActivityDataByName(name: string): Observable<any> {
    const headers = {};
    headers['responseType'] = 'arrayBuffer';
    headers['headers'] = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/pdf'
    });

    const url = this.apiReportsUrl + `/${name}`;
    return this.httpClient.get<any>(url, headers);
  }

  public getBulkInvoiceByInfo(reportInfo: Object): Observable<any> {
    let url = this.apiReportsUrl;
    const headers = {};
    headers['headers'] = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    });

    url += '?Type=BulkInvoice';
    if (reportInfo['SchoolYear']) { url += `&SchoolYear=${reportInfo['SchoolYear']}`; }
    if (reportInfo['Scope']) { url += `&Scope=${reportInfo['Scope']}`; }

    return this.httpClient.get<any>(url, headers);
  }

  // HTTP GET /api/activity/bulk
  public getBulkActivity(invoiceInfo: Object): Observable<Report> {
    invoiceInfo = Object.assign(invoiceInfo, { 'reportType': 'StudentInformation' });
    const url = this.apiReportsUrl + '/bulk';
    return this.httpClient.post<any>(url, invoiceInfo, this.headers);
  }

  public getActivities(name: string, year: string, scope: string, approved: boolean): Observable<Report[]> {
    const reportInfo: Object = Object.assign({}, { 'Type': ReportType.BulkStudentInformation });

    if (name) { reportInfo['Name'] = name; }
    if (year) { reportInfo['SchoolYear'] = year; }
    if (approved != null) { reportInfo['Approved'] = approved; }
    if (scope) { reportInfo['Scope'] = scope; }

    return this.getReportsByMeta(reportInfo);
  }

  // HTTP POST /api/reports/many
  public createInvoices(invoiceInfo: Object): Observable<Report[]> {
    const url = this.apiReportsUrl + '/many';
    return this.httpClient.post<any>(url, invoiceInfo, this.headers);
  }

  // HTTP POST /api/reports/
  public createInvoice(invoiceInfo: Object): Observable<Report> {
    const url = this.apiReportsUrl;
    return this.httpClient.post<any>(url, invoiceInfo, this.headers);
  }

  // HTTP POST /api/reports
  public createBulkInvoice(invoiceInfo: Object): Observable<Report> {
    const url = this.apiReportsUrl;
    return this.httpClient.post<any>(url, invoiceInfo, this.headers);
  }

  // HTTP POST /api/Reports
  public createBulkActivity(activityInfo: Object): Observable<Report> {
    activityInfo = Object.assign(activityInfo,
      {
        'reportType': 'BulkStudentInformation',
      });
    const url = this.apiReportsUrl;
    return this.httpClient.post<any>(url, activityInfo, this.headers);

    // return Observable.of(null);
  }

  public createAccountsReceivableAsOf(name: string, schoolYear: string, asOf: Date, auns?: number[]): Observable<Report> {
    const url: string = this.apiReportsUrl;
    const reportMeta: Object = Object.assign({}, {
      'reportType': ReportType.AccountsReceivableAsOf,
      'name': name,
      'schoolYear': schoolYear.replace(/\s+/g, ''),
      'accountsReceivableAsOf': {
        'asOf': asOf.toLocaleDateString('en-US'),
        'auns': auns
      }
    });
    return this.httpClient.post<any>(url, reportMeta, this.headers);
  }

  public createCSIU(name: string, asOf: Date, auns?: number[]): Observable<Report> {
    const url: string = this.apiReportsUrl;
    const reportMeta: Object = Object.assign({}, {
      'reportType': ReportType.CSIU,
      // 'schoolYear': schoolYear.replace(/\s+/g, ''),
      'name': name,
      'csiu': {
        'asOf': asOf.toLocaleDateString('en-US'),
        'auns': auns
      }
    });

    return this.httpClient.post<any>(url, reportMeta, this.headers);
  }
}
