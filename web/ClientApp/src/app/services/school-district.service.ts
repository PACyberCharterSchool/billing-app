import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { SchoolDistrict } from '../models/school-district.model';

@Injectable()
export class SchoolDistrictService {
  private apiSchoolDistrictsUrl: string;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(
    private httpClient: HttpClient) {
    this.apiSchoolDistrictsUrl = '/api/schoolDistricts';
  }

  public getSchoolDistricts(): Observable<SchoolDistrict[]> {
    const url = this.apiSchoolDistrictsUrl;
    return this.httpClient.get<SchoolDistrict[]>(this.apiSchoolDistrictsUrl, this.headers);
  }

  public getSchoolDistrict(id: number): Observable<SchoolDistrict> {
    return this.httpClient.get<SchoolDistrict>(this.apiSchoolDistrictsUrl + `/${id}`, this.headers);
  }

  public updateSchoolDistrict(schoolDistrict: SchoolDistrict): Observable<SchoolDistrict> {
    if (schoolDistrict) {
      const reqBodyObj = this.buildRequestBodyObject(schoolDistrict);
      return this.httpClient.put<SchoolDistrict>(
        this.apiSchoolDistrictsUrl + `/${schoolDistrict.id}`,
        reqBodyObj,
        this.headers
      );
    }
  }

  private buildRequestBodyObject(sd: SchoolDistrict): Object {
    const reqBodyObj = Object.assign({}, {
      aun: sd.aun,
      name: sd.name,
      rate: sd.rate,
      alternateRate: sd.alternateRate,
      paymentRate: sd.alternateRate,
      paymentType: sd.paymentType
    });

    return reqBodyObj;
  }
}
