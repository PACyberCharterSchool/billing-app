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

  public updateSchoolDistricts(schoolDistrictData: FormData): Observable<SchoolDistrict[]> {
    const url = this.apiSchoolDistrictsUrl + `?file=${schoolDistrictData}`;
    return this.httpClient.put<any>(url, this.serializeSchoolDistrictRequestBodyObject(schoolDistrictData));
  }

  private buildRequestBodyObject(sd: SchoolDistrict): Object {
    const reqBodyObj = Object.assign({}, {
      aun: sd.aun,
      name: sd.name,
      rate: +sd.rate,
      alternateRate: +sd.alternateRate,
      specialEducationRate: +sd.specialEducationRate,
      alternateSpecialEducationRate: +sd.alternateSpecialEducationRate,
      paymentType: sd.paymentType === 'Check' ? 'Check' : 'UniPay'
    });

    if (+sd.alternateRate === 0.0) {
      delete reqBodyObj.alternateRate;
    }

    if (+sd.alternateSpecialEducationRate === 0.0) {
      delete reqBodyObj.alternateSpecialEducationRate;
    }

    return reqBodyObj;
  }

  private serializeSchoolDistrictRequestBodyObject(schoolDistrictData: FormData): Object {
    Object.keys(schoolDistrictData).forEach(
      k => {
        const v = schoolDistrictData[k];
        if (typeof(v) === 'object') {
          schoolDistrictData.set(k, JSON.stringify(v));
        } else {
          schoolDistrictData.set(k, v);
        }
      }
    );

    return schoolDistrictData;
  }
}
