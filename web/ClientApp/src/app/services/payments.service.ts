import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Payment } from '../models/payment.model';

import { Globals } from '../globals';

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
    const url = this.apiPaymentsUrl + `?skip=${skip}&take=${this.globals.take}`;
    return this.httpClient.get<Payment[]>(url, this.headers);
  }

  public createPayment(payment: Payment): Observable<Payment> {
    const url = this.apiPaymentsUrl;
    const reqBody = this.buildPaymentRequestBodyObject(payment);
    return this.httpClient.post<Payment>(url, reqBody, this.headers);
  }

  public updatePayment(payment: Payment): Observable<Payment> {
    const url = this.apiPaymentsUrl + `/${payment.paymentId}`;
    const reqBody = this.buildPaymentRequestBodyObject(payment);
    return this.httpClient.put<Payment>(url, reqBody, this.headers);
  }

  private buildPaymentRequestBodyObject(payment: Payment) {
    const requestObject = Object.assign({}, {
      id: payment.paymentId,
      splits: this.fillSplitsColumn(payment),
      date: payment.date,
      externalId: payment.externalId,
      type: payment.type,
      schoolDistrictAun: +payment.schoolDistrict.aun
    });

    return requestObject;
  }

  private fillSplitsColumn(payment: Payment) {
    let splits: Object[];

    splits = [{ 'amount': payment.amount, 'schoolYear': payment.schoolYear.replace(/\s/g, '') }];
    if (payment.splitAmount) {
      splits.push({ 'amount': payment.splitAmount, 'schoolYear': payment.schoolYearSplit });
    }

    return splits;
  }
}
