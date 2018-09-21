import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

// import { Observable } from 'rxjs';
import { Observable } from 'rxjs/Observable';

import { AuditRecord, AuditRecordActivityType } from '../models/audit-record.model';

import { Globals } from '../globals';

@Injectable()
export class AuditRecordsService {
  private readonly apiAuditRecordsUrl;
  private readonly headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(
    private globals: Globals,
    private httpClient: HttpClient
  ) {
    this.apiAuditRecordsUrl = '/api/Audits';
  }

  getAll(skip: number): Observable<AuditRecord[]> {
    const url = this.apiAuditRecordsUrl;
    return this.httpClient.get<AuditRecord[]>(url, this.headers);
    // return Observable.of(auditRecords);
  }
}
