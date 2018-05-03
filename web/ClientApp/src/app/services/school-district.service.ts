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

  constructor(private httpClient: HttpClient) {
    this.apiSchoolDistrictsUrl = 'http://localhost:5000/api/schoolDistricts';
  }

  public getSchoolDistricts(): Observable<SchoolDistrict[]> {
    return this.httpClient.get<SchoolDistrict[]>(this.apiSchoolDistrictsUrl, this.headers);
  }

  public getSchoolDistrict(id: number): Observable<SchoolDistrict> {
    console.log('SchoolDistrictService.getSchoolDistrict():  id is ', id);
    return this.httpClient.get<SchoolDistrict>(this.apiSchoolDistrictsUrl + `/${id}`, this.headers);
  }

  public updateSchoolDistrict(schoolDistrict: SchoolDistrict): Observable<SchoolDistrict> {
    console.log('SchoolDistrictService.updateSchoolDistrict():  schoolDistrict is ', schoolDistrict);
    if (schoolDistrict) {
      return this.httpClient.put<SchoolDistrict>(
        this.apiSchoolDistrictsUrl + `/${schoolDistrict.id}`,
        { 'schoolDistrict': schoolDistrict },
        this.headers
      );
    }
  }
}
