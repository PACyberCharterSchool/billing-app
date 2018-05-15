import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Refund } from '../models/refund.model';

let refunds: Refund[] = [
  {
    schoolDistrictName: 'Seneca Valley SD',
    schoolDistrictId: 123456,
    refundAmt: 4300.00,
    refundCheckNumber: '#14534',
    refundDate: new Date('04/01/2018'),
    academicYear: '2018-2019'
  },
  {
    schoolDistrictName: 'Mars SD',
    schoolDistrictId: 654321,
    refundAmt: 1800.00,
    refundCheckNumber: '#99999',
    refundDate: new Date('07/22/2018'),
    academicYear: '2018-2019'
  },
  {
    schoolDistrictName: 'Indiana SD',
    schoolDistrictId: 789023,
    refundAmt: 40000.00,
    refundCheckNumber: '#99999',
    refundDate: new Date('07/22/2018'),
    academicYear: '2018-2019'
  }
];
@Injectable()
export class RefundsService {
  private apiRefundsUrl;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor() {
    this.apiRefundsUrl = 'api/refunds';
  }

  public getRefunds(): Observable<Refund[]> {
    // just return some fake data for now
    return new Observable<Refund[]>((o) => {
      o.next(refunds);
      o.complete();
    });
  }

  public createRefund(refund: Refund): Observable<Refund> {
    // just return some fake data for now
    return new Observable<Refund>((o) => {
      refunds.push(refund);
      o.next(refund);
      o.complete();
    });
  }

  public updateRefund(refund: Refund): Observable<Refund> {
    // just return some fake data for now
    return new Observable<Refund>((o) => {
      o.next(refund);
      o.complete();
    });
  }
}
