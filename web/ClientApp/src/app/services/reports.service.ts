import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response, ResponseContentType } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Report, ReportType } from '../models/report.model';
import { Template } from '../models/template.model';

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
  public getReportsByInfo(reportInfo: Object): Observable<Report[]> {
    let url = this.apiReportsUrl;

    if (reportInfo['Type']) { url +=  `?Type=${reportInfo['Type']}`; } // there will *always* be a Type
    if (reportInfo['Name']) { url += `&Name=${reportInfo['Name']}`; }
    if (reportInfo['SchoolYear']) { url += `&SchoolYear=${reportInfo['SchoolYear']}`; }
    if (reportInfo['Approved']) { url += `&Approved=${reportInfo['Approved']}`; }
    if (reportInfo['Scope']) { url += `&Scope=${reportInfo['Scope']}`; }

    return this.httpClient.get<Report[]>(url, this.headers);
  }

  // HTTP Get /api/reports/{id}
  public getReport(id: number): Observable<Report> {
    const url = this.apiReportsUrl + `/${id}`;
    return this.httpClient.get<Report>(url, this.headers);
  }

  // HTTP GET /api/reports/activity/name
  public getReportStudentActivityDataByFormat(report: Report, format: string): Observable<any> {
    const url = this.apiReportsUrl + `/activity/name?Name=${report.name}&Format=${format}`;
    const headers = {};
    headers['responseType'] = 'arrayBuffer';
    headers['headers'] = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/pdf'
    });

    return this.httpClient.get<any>(url, headers);
  }

  // HTTP GET /api/reports/invoice/name
  public getReportInvoiceByDataFormat(report: Report, format: string): Observable<ArrayBuffer> {
    const url = this.apiReportsUrl + `/invoice/name?Name=${report.name}&Format=${format}`;
    const headers = {};
    headers['responseType'] = 'arrayBuffer';
    headers['headers'] = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/pdf'
    });

    return this.httpClient.get<ArrayBuffer>(url, headers);
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

  // HTTP POST /api/reports/bulk
  public createBulkInvoice(invoiceInfo: Object): Observable<Report> {
    invoiceInfo = Object.assign(invoiceInfo, { 'reportType': 'BulkInvoice' });
    const url = this.apiReportsUrl + '/bulk';
    return this.httpClient.post<any>(url, invoiceInfo, this.headers);
  }

  public getInvoices(name: string, year: string, scope: string, approved: boolean): Observable<Report[]> {
    const reportInfo: Object = Object.assign({}, {'Type': ReportType.Invoice});

    if (name) { reportInfo['Name'] = name; }
    if (year) { reportInfo['SchoolYear'] = year; }
    if (approved != null) { reportInfo['Approved'] = approved; }
    if (scope) { reportInfo['Scope'] = scope; }

    return this.getReportsByInfo(reportInfo);
  }

  public getInvoicesZipped(name: string, year: string, approved: boolean): Observable<any> {
    const url = this.apiReportsUrl + '/zip';
    return this.httpClient.get<any>(url, this.headers);
  }

  public getBulkInvoices(year: string): Observable<any> {
    const reportInfo: Object = Object.assign({}, {'Type': ReportType.BulkInvoice});

    if (year) { reportInfo['SchoolYear'] = year; }

    return this.getBulkInvoiceByInfo(reportInfo);
  }

  public getInvoiceByName(name: string): Observable<any> {
    const headers = {};
    headers['responseType'] = 'arrayBuffer';
    headers['headers'] = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    });

    const url = this.apiReportsUrl + `/${name}`;
    return this.httpClient.get<any>(url, headers);
  }

  public getInvoicesBySchoolYearAndStatus(year: string, status: string): Observable<Report[]> {
    const url = this.apiReportsUrl + `?year=${year}&status=${status}`;
    return this.httpClient.get<Report[]>(url, this.headers);
  }

  public getInvoiceStudentActivityDataBulk(year: string, approved: boolean): Observable<any> {
    const reportInfo: Object = Object.assign({}, {'Type': ReportType.Invoice});

    if (year) { reportInfo['SchoolYear'] = year; }
    if (approved != null) { reportInfo['Approved'] = approved; }

    return this.getInvoiceStudentActivityDataBulkByInfo(reportInfo);
  }

  public getInvoiceStudentActivityDataByName(name: string): Observable<any> {
    const headers = {};
    headers['responseType'] = 'arrayBuffer';
    headers['headers'] = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    });

    const url = this.apiReportsUrl + `/activity/name/${name}`;
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

    return this.httpClient.get<any>(url, headers);
  }

  public getInvoiceStudentActivityDataBulkByInfo(reportInfo: Object): Observable<any> {
    let url = this.apiReportsUrl + '/activity';
    const headers = {};
    headers['responseType'] = 'arrayBuffer';
    headers['headers'] = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    });

    url += '?Type=Invoice';
    if (reportInfo['SchoolYear']) { url += `&SchoolYear=${reportInfo['SchoolYear']}`; }
    url += reportInfo['Approved'] ? `&Approved=true` : `&Approved=false`;

    return this.httpClient.get<any>(url, headers);
  }

  public getReportByName(name: string): Observable<Report> {
    const url = this.apiReportsUrl + `/${name}`;
    return this.httpClient.get<Report>(url, this.headers);
  }

  // HTTP POST /api/activity/bulk
  public createBulkActivity(activityInfo: Object): Observable<Report> {
    return Observable.of(null);
  }

  // HTTP GET /api/activity/bulk
  public getBulkActivity(invoiceInfo: Object): Observable<Report> {
    invoiceInfo = Object.assign(invoiceInfo, { 'reportType': 'StudentInformation' });
    const url = this.apiReportsUrl + '/bulk';
    return this.httpClient.post<any>(url, invoiceInfo, this.headers);
  }
}
