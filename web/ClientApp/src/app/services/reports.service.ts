import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Report, ReportType } from '../models/report.model';
import { Globals } from '../globals';
import { map } from 'rxjs/operators';
import { UtilitiesService } from './utilities.service';

export class ReportsResponse {
  reports: Report[];
}

class ReportInfo {
  type: string;
  name?: string;
  schoolYear?: string;
  approved?: boolean;
  scope?: string;
}

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

  private convertReport(r: Report): Report {
    r.created = UtilitiesService.convertDate(r.created);
    return r;
  }

  // HTTP GET /api/reports
  public getReportsByMeta(reportInfo: ReportInfo): Observable<ReportsResponse> {
    let url = this.apiReportsUrl;

    url += `?Type=${reportInfo.type}`;
    if (reportInfo.name) {
      url += `&Name=${reportInfo.name}`;
    }
    if (reportInfo.schoolYear) {
      url += `&SchoolYear=${reportInfo.schoolYear}`;
    }
    if (reportInfo.approved) {
      url += `&Approved=${reportInfo.approved}`;
    }
    if (reportInfo.scope) {
      url += `&Scope=${reportInfo.scope}`;
    }

    return this.httpClient.get<ReportsResponse>(url, this.headers).pipe(map(res => {
      return { reports: res.reports.map(this.convertReport) };
    }));
  }

  public getAccountsReceivableAsOf(): Observable<ReportsResponse> {
    return this.getReportsByMeta({ type: ReportType.AccountsReceivableAsOf });
  }

  public getAccountsReceivableAging(): Observable<ReportsResponse> {
    return this.getReportsByMeta({ type: ReportType.AccountsReceivableAging });
  }

  public getCSIU(): Observable<ReportsResponse> {
    return this.getReportsByMeta({ type: ReportType.CSIU });
  }

  public getTotalsOnly(): Observable<ReportsResponse> {
    return this.getReportsByMeta({ type: ReportType.TotalsOnly });
  }

  public getUniPayInvoiceSummary(): Observable<ReportsResponse> {
    return this.getReportsByMeta({ type: ReportType.UniPayInvoiceSummary });
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

  public getInvoices(name?: string, year?: string, scope?: string, approved?: boolean): Observable<ReportsResponse> {
    return this.getReportsByMeta({
      type: ReportType.Invoice,
      name: name,
      schoolYear: year,
      approved: approved,
      scope: scope,
    });
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

  public getActivities(name?: string, year?: string, scope?: string, approved?: boolean): Observable<ReportsResponse> {
    return this.getReportsByMeta({
      type: ReportType.BulkStudentInformation,
      name: name,
      schoolYear: year,
      approved: approved,
      scope: scope,
    });
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

  public createAccountsReceivableAging(name: string, from: Date, asOf: Date, auns?: number[]): Observable<Report> {
    const url: string = this.apiReportsUrl;
    const reportMeta: Object = Object.assign({}, {
      'reportType': ReportType.AccountsReceivableAging,
      'name': name,
      'accountsReceivableAging': {
        'from': from ? from.toLocaleDateString('en-US') : undefined,
        'asOf': asOf ? asOf.toLocaleDateString('en-US') : undefined,
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

  public createTotalsOnlyInvoice(
    name: string,
    scope: string,
    schoolYear: string,
    paymentType: string,
    auns?: number[]
  ): Observable<Report> {
    const url: string = this.apiReportsUrl;
    const reportMeta: Object = Object.assign({}, {
      'reportType': ReportType.TotalsOnly,
      'schoolYear': schoolYear.replace(/\s+/g, ''),
      'name': name,
      'totalsOnlyInvoice': {
        'scope': scope,
        'auns': auns,
        'paymentType': paymentType
      }
    });
    return this.httpClient.post<any>(url, reportMeta, this.headers);
  }

  public createUniPayInvoiceSummary(name: string, schoolYear: string, asOf: Date): Observable<Report> {
    const url: string = this.apiReportsUrl;
    const reportMeta: Object = Object.assign({}, {
      'reportType': ReportType.UniPayInvoiceSummary,
      'schoolYear': schoolYear.replace(/\s+/g, ''),
      'name': name,
      'uniPayInvoiceSummary': {
        'asOf': new Date(Date.now()).toLocaleDateString('en-US')
      }
    });

    return this.httpClient.post<any>(url, reportMeta, this.headers);
  }
}
