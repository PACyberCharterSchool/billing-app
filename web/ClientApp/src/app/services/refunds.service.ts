import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Refund } from '../models/refund.model';

import { Globals } from '../globals';

@Injectable()
export class RefundsService {
  private apiRefundsUrl;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private globals: Globals, private httpClient: HttpClient) {
    this.apiRefundsUrl = 'api/Refunds';
  }

  public getRefunds(skip: number): Observable<Refund[]> {
    const url = this.apiRefundsUrl + `?skip=${skip}&take=${this.globals.take}`;
    return this.httpClient.get<Refund[]>(url, this.headers);
  }

  public createRefund(refund: Refund): Observable<Refund> {
    const url = this.apiRefundsUrl;
    const reqBodyObj = this.buildRefundRequestBodyObject(refund);
    return this.httpClient.post<Refund>(url, reqBodyObj, this.headers);
  }

  public updateRefund(refund: Refund): Observable<Refund> {
    const url = this.apiRefundsUrl + `/${refund.id}`;
    const reqBodyObj = this.buildRefundRequestBodyObject(refund);
    return this.httpClient.put<Refund>(url, reqBodyObj, this.headers);
  }

  private buildRefundRequestBodyObject(refund: Refund) {
    const reqBodyObj = Object.assign({}, {
      amount: refund.amount,
      checkNumber: refund.checkNumber,
      date: refund.date,
      schoolYear: refund.schoolYear,
      schoolDistrictAun: +refund.schoolDistrict.aun
    });

    return reqBodyObj;
  }
}
