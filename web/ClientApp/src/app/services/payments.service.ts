import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Payment } from '../models/payment.model';

import { Globals } from '../globals';

// let payments: Payment[] = [
//   {
//     schoolDistrictName: 'Seneca Valley SD',
//     schoolDistrictId: 123456,
//     paymentAmt: 4300.00,
//     type: '#14534',
//     paymentDate: new Date('04/01/2018'),
//     academicYear: '2018-2019'
//   },
//   {
//     schoolDistrictName: 'Mars SD',
//     schoolDistrictId: 654321,
//     paymentAmt: 1800.00,
//     type: '#99999',
//     paymentDate: new Date('07/22/2018'),
//     academicYear: '2018-2019'
//   },
//   {
//     schoolDistrictName: 'Indiana SD',
//     schoolDistrictId: 789023,
//     paymentAmt: 40000.00,
//     type: '#99999',
//     paymentDate: new Date('07/22/2018'),
//     academicYear: '2018-2019'
//   }
// ];

@Injectable()
export class PaymentsService {
  private apiPaymentsUrl;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private globals: Globals, private httpClient: HttpClient) {
    this.apiPaymentsUrl = 'api/Payments';
  }

  public getPayments(skip: number): Observable<Payment[]> {
    // just return some fake data for now
    const url = this.apiPaymentsUrl + `?skip=${skip}&take=${this.globals.take}`;
    return this.httpClient.get<Payment[]>(url, this.headers);
  }

  public createPayment(payment: Payment): Observable<Payment> {
    // just return some fake data for now
    const url = this.apiPaymentsUrl;
    return this.httpClient.post<Payment>(url, payment, this.headers);
  }

  public updatePayment(payment: Payment): Observable<Payment> {
    // just return some fake data for now
    const url = this.apiPaymentsUrl;
    return this.httpClient.put<Payment>(url, payment, this.headers);
  }
}
