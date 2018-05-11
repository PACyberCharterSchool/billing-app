import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Payment } from '../models/payment.model';

let payments: Payment[] = [
  {
    schoolDistrictName: 'Seneca Valley SD',
    schoolDistrictId: 123456,
    paymentAmt: 4300.00,
    type: '#14534',
    paymentDate: new Date('04/01/2018'),
    academicYear: '2018-2019'
  },
  {
    schoolDistrictName: 'Mars SD',
    schoolDistrictId: 654321,
    paymentAmt: 1800.00,
    type: '#99999',
    paymentDate: new Date('07/22/2018'),
    academicYear: '2018-2019'
  },
  {
    schoolDistrictName: 'Indiana SD',
    schoolDistrictId: 789023,
    paymentAmt: 40000.00,
    type: '#99999',
    paymentDate: new Date('07/22/2018'),
    academicYear: '2018-2019'
  }
];

@Injectable()
export class PaymentsService {
  private apiPaymentsUrl;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor() {
    this.apiPaymentsUrl = 'api/payments';
  }

  public getPayments(): Observable<Payment[]> {
    // just return some fake data for now
    return new Observable<Payment[]>((o) => {
      o.next(payments);
      o.complete();
    });
  }

  public createPayment(payment: Payment): Observable<Payment> {
    // just return some fake data for now
    return new Observable<Payment>((o) => {
      payments.push(payment);
      o.next(payment);
      o.complete();
    });
  }

  public updatePayment(payment: Payment): Observable<Payment> {
    // just return some fake data for now
    return new Observable<Payment>((o) => {
      o.next(payment);
      o.complete();
    });
  }
}
