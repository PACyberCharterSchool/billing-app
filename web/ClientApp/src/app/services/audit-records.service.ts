import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Rx';

import { AuditRecord, AuditRecordType } from '../models/audit-record.model';

import { Globals } from '../globals';

let auditRecords: AuditRecord[] = [
  {
    id: 1,
    username: 'RA2',
    activity: 'Template Invoice Change',
    type: AuditRecordType.InvoiceTemplates,
    timestamp: new Date('Wed May 23 13:31:28 EDT 2018'),
    oldValue: 'Individual Student 2017-2018',
    newValue: 'Individual Student 2018-2019'
  },
  {
    id: 2,
    username: 'RA2',
    activity: 'School District Change',
    type: AuditRecordType.SchoolDistricts,
    timestamp: new Date ('Fri May 25 13:35:43 EDT 2018'),
    oldValue: 'Seneca Valley SD',
    newValue: 'Mars SD'
  },
  {
    id: 3,
    username: 'RA1',
    activity: 'School District Change',
    type: AuditRecordType.InvoiceTemplates,
    timestamp: new Date('Sat May 26 13:31:28 EDT 2018'),
    oldValue: 'Penn Trafford SD',
    newValue: 'Gateway SD'
  },
  {
    id: 4,
    username: 'RA1',
    activity: 'Template Invoice Change',
    type: AuditRecordType.InvoiceTemplates,
    timestamp: new Date('Thu May 24 13:31:28 EDT 2018'),
    oldValue: 'Individual UniPay 2017-2018',
    newValue: 'Individual UniPay 2018-2019'
  },
  {
    id: 5,
    username: 'RA3',
    activity: 'School District Change',
    type: AuditRecordType.SchoolDistricts,
    timestamp: new Date ('Fri May 25 13:35:43 EDT 2018'),
    oldValue: 'Tionesta SD',
    newValue: 'Happy Valley SD'
  },
  {
    id: 6,
    username: 'RA2',
    activity: 'School District Change',
    type: AuditRecordType.InvoiceTemplates,
    timestamp: new Date('Mon May 21 13:31:28 EDT 2018'),
    oldValue: 'Ellwood City SD',
    newValue: 'Carkundle SD'
  }
];

@Injectable()
export class AuditRecordsService {

  private readonly apiAuditRecordsUrl;

  constructor(
    private globals: Globals,
    private httpClient: HttpClient
  ) {
    this.apiAuditRecordsUrl = '/api/Audits';
  }

  getAll(skip: number): Observable<AuditRecord[]> {
    const url = this.apiAuditRecordsUrl + `?skip=${skip}&take=${this.globals.take}`;
    return Observable.of(auditRecords);
  }
}
