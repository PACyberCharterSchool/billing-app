import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Payment } from '../models/payment.model';
import { Globals } from '../globals';
import { map } from 'rxjs/operators';
import { UtilitiesService } from './utilities.service';

export class PaymentsResponse {
  payments: Payment[];
}

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

  private convertPayment(p: Payment): Payment {
    p.date = UtilitiesService.convertDate(p.date);
    p.created = UtilitiesService.convertDate(p.created);
    p.lastUpdated = UtilitiesService.convertDate(p.lastUpdated);
    return p;
  }

  public getPayments(skip: number): Observable<PaymentsResponse> {
    const url = this.apiPaymentsUrl + `?skip=${skip}&take=${this.globals.take}`;
    return this.httpClient.get<PaymentsResponse>(url, this.headers).pipe(map(res => {
      return { payments: res.payments.map(this.convertPayment) };
    }));
  }

  public getPaymentsByPaymentId(paymentId: string): Observable<PaymentsResponse> {
    const url = this.apiPaymentsUrl + `/${paymentId}`;
    return this.httpClient.get<PaymentsResponse>(url, this.headers);
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

  public updatePDEPayments(date: Date, paymentData: FormData): Observable<Payment[]> {
    let url = this.apiPaymentsUrl + `?file=${paymentData}`;
    if (date) {
      url += `&date=${date.toLocaleDateString('en-US')}`;
    }
    return this.httpClient.put<any>(url, this.serializePDEPaymentRequestBodyObject(paymentData));
  }

  private serializePDEPaymentRequestBodyObject(paymentData: FormData): Object {
    Object.keys(paymentData).forEach(
      k => {
        const v = paymentData[k];
        if (typeof (v) === 'object') {
          paymentData.set(k, JSON.stringify(v));
        } else {
          paymentData.set(k, v);
        }
      }
    );

    return paymentData;
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

    splits = [{ 'date': payment.date, 'amount': payment.amount, 'schoolYear': payment.schoolYear.replace(/\s/g, '') }];
    if (payment.splitAmount) {
      splits.push({ 'date': payment.date, 'amount': payment.splitAmount, 'schoolYear': payment.schoolYearSplit });
    }

    return splits;
  }
}
