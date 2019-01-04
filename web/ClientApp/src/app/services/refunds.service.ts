import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Refund } from '../models/refund.model';
import { Globals } from '../globals';
import { map } from 'rxjs/operators';
import { UtilitiesService } from './utilities.service';

export class RefundsResponse {
  refunds: Refund[];
}

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

  private convertRefund(r: Refund): Refund {
    r.date = UtilitiesService.convertDate(r.date);
    r.created = UtilitiesService.convertDate(r.created);
    r.lastUpdated = UtilitiesService.convertDate(r.lastUpdated);
    return r;
  }

  public getRefunds(skip: number): Observable<RefundsResponse> {
    const url = this.apiRefundsUrl + `?skip=${skip}&take=${this.globals.take}`;
    return this.httpClient.get<RefundsResponse>(url, this.headers).pipe(map(res => {
      return { refunds: res.refunds.map(this.convertRefund) };
    }));
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
      schoolYear: refund.schoolYear.replace(/\s+/g, ''),
      schoolDistrictAun: +refund.schoolDistrict.aun
    });

    return reqBodyObj;
  }
}
