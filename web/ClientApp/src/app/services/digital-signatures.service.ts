import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { DigitalSignature } from '../models/digital-signature.model';

import { Globals } from '../globals';

@Injectable()
export class DigitalSignaturesService {
  private apiDigitalSignatureUrl: string;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application-json'
    })
  }

  constructor(
    private globals: Globals,
    private httpClient: HttpClient
  ) {
    this.apiDigitalSignatureUrl = 'api/DigitalSignatures';
  }

  public getDigitalSignatures(skip: number): Observable<DigitalSignature[]> {
    const url = this.apiDigitalSignatureUrl + `?skip=${skip}&take=${this.globals.take}`;
    return this.httpClient.get<DigitalSignature[]>(url, this.headers);
  }

  public getDigitalSignature(id: number): Observable<DigitalSignature> {
    const url = this.apiDigitalSignatureUrl + `/${id}`;
    return this.httpClient.get<DigitalSignature>(url, this.headers);
  }

  public createDigitalSignature(signatureData: FormData): Observable<DigitalSignature> {
    const url = this.apiDigitalSignatureUrl;
    const reqBodyObj = this.serializeSignatureRequestBodyObject(signatureData);
    console.log('DigitalSignaturesService.createDigitalSignature():  signatureData is ', signatureData);
    return this.httpClient.post<any>(url, reqBodyObj);
  }

  public deleteDigitalSignature(id: number): Observable<DigitalSignature> {
    const url = this.apiDigitalSignatureUrl + `/${id}`;
    return this.httpClient.delete<DigitalSignature>(url, this.headers);
  }

  private serializeSignatureRequestBodyObject(signatureData: FormData): Object {
    Object.keys(signatureData).forEach(
      k => {
        let v = signatureData[k];
        if (typeof(v) === 'object') {
          signatureData.set(k, JSON.stringify(v));
        }
        else {
          signatureData.set(k, v);
        }
      }
    )

    return signatureData;
  }
}
