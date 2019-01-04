import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { SchoolDistrict } from '../models/school-district.model';
import { UtilitiesService } from './utilities.service';
import { map } from 'rxjs/operators';

export class SchoolDistrictResponse {
  schoolDistrict: SchoolDistrict;
}

export class SchoolDistrictsResponse {
  schoolDistricts: SchoolDistrict[];
}

@Injectable()
export class SchoolDistrictService {
  private apiSchoolDistrictsUrl: string;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(
    private httpClient: HttpClient,
  ) {
    this.apiSchoolDistrictsUrl = '/api/schoolDistricts';
  }

  private convertSchoolDistrict(d: SchoolDistrict): SchoolDistrict {
    d.created = UtilitiesService.convertDate(d.created);
    d.lastUpdated = UtilitiesService.convertDate(d.lastUpdated);
    return d;
  }

  public getSchoolDistricts(): Observable<SchoolDistrictsResponse> {
    return this.httpClient.get<SchoolDistrictsResponse>(this.apiSchoolDistrictsUrl, this.headers).
      pipe(map(res => {
        return { schoolDistricts: res.schoolDistricts.map(this.convertSchoolDistrict) };
      }));
  }

  public getSchoolDistrict(id: number): Observable<SchoolDistrictResponse> {
    return this.httpClient.get<SchoolDistrictResponse>(this.apiSchoolDistrictsUrl + `/${id}`, this.headers).
      pipe(map(res => {
        return { schoolDistrict: this.convertSchoolDistrict(res.schoolDistrict) };
      }));
  }

  public updateSchoolDistrict(schoolDistrict: SchoolDistrict): Observable<SchoolDistrictResponse> {
    const reqBodyObj = this.buildRequestBodyObject(schoolDistrict);
    return this.httpClient.put<SchoolDistrictResponse>(
      this.apiSchoolDistrictsUrl + `/${schoolDistrict.id}`,
      reqBodyObj,
      this.headers).pipe(map(res => {
        return { schoolDistrict: this.convertSchoolDistrict(res.schoolDistrict) };
      }));
  }

  public updateSchoolDistricts(schoolDistrictData: FormData): Observable<SchoolDistrictsResponse> {
    const url = this.apiSchoolDistrictsUrl + `?file=${schoolDistrictData}`;
    return this.httpClient.put<SchoolDistrictsResponse>(
      url,
      this.serializeSchoolDistrictRequestBodyObject(schoolDistrictData)).pipe(map(res => {
        return { schoolDistricts: res.schoolDistricts.map(this.convertSchoolDistrict) };
      }));
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
        if (typeof (v) === 'object') {
          schoolDistrictData.set(k, JSON.stringify(v));
        } else {
          schoolDistrictData.set(k, v);
        }
      }
    );

    return schoolDistrictData;
  }
}
