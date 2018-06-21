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

  public getInvoices(): Observable<Report[]> {
    return this.getReportsByType('Invoice');
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

  public getReportsByType(type: string): Observable<Report[]> {
    const url = this.apiReportsUrl + `?Type=${type}`;
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
