import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Response, ResponseContentType } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

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

  public getReports(skip: number): Observable<Report[]> {
    const url = this.apiReportsUrl;
    return this.httpClient.get<Report[]>(url, this.headers);
  }

  public getInvoices(name: string, year: string, approved: boolean): Observable<Report[]> {
    let reportInfo: Object = Object.assign({}, {'Type': ReportType.Invoice});

    if (name) reportInfo['Name'] = name;
    if (year) reportInfo['SchoolYear'] = year;
    if (approved != null) reportInfo['Appoved'] = approved;

    return this.getReportsByInfo(reportInfo);
  }

  public getInvoicesZipped(name: string, year: string, approved: boolean): Observable<any> {
    const url = this.apiReportsUrl + '/zip';
    return this.httpClient.get<any>(url, this.headers);
  }

  public getInvoiceByName(name: string): Observable<any> {
    let headers = {};
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

  public getInvoiceActivityDataByName(name: string): Observable<any> {
    let headers = {};
    headers['responseType'] = 'arrayBuffer';
    headers['headers'] = new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    });

    const url = this.apiReportsUrl + `/activity/${name}`;
    return this.httpClient.get<any>(url, headers);
  }

  public createInvoices(invoiceInfo: Object): Observable<Report[]> {
    const url = this.apiReportsUrl + '/many';
    return this.httpClient.post<any>(url, invoiceInfo, this.headers);
  }

  public createInvoice(invoiceInfo: Object): Observable<Report> {
    const url = this.apiReportsUrl;
    return this.httpClient.post<any>(url, invoiceInfo, this.headers);
  }

  public getReportsByInfo(reportInfo: Object): Observable<Report[]> {
    let url = this.apiReportsUrl;
    if (reportInfo['Type']) url +=  `?Type=${reportInfo['Type']}`;
    if (reportInfo['Name']) url += `&Name=${reportInfo['Name']}`;
    if (reportInfo['SchoolYear']) url += `&SchoolYear=${reportInfo['SchoolYear']}`;
    if (reportInfo['Approved']) url += `&Approved=${reportInfo['Approved']}`;
    return this.httpClient.get<Report[]>(url, this.headers);
  }

  public getReport(id: number): Observable<Report> {
    const url = this.apiReportsUrl + `/${id}`;
    return this.httpClient.get<Report>(url, this.headers);
  }

  public getReportByName(name: string): Observable<Report> {
    const url = this.apiReportsUrl + `/${name}`;
    return this.httpClient.get<Report>(url, this.headers);
  }
}
