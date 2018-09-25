import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Calendar } from '../models/calendar.model';

@Injectable()
export class SchoolCalendarService {

  private apiSchoolCalendarUrl;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private httpClient: HttpClient) {
    this.apiSchoolCalendarUrl = '/api/Calendars';
  }

  getAcademicYears(): Observable<string[]> {
    const url = this.apiSchoolCalendarUrl + '/years';
    return this.httpClient.get<string[]>(url, this.headers);
  }

  getByYear(year: string): Observable<Calendar> {
    const url = this.apiSchoolCalendarUrl + `/${year.replace(/\s+/g, '')}`;
    return this.httpClient.get<Calendar>(url, this.headers);
  }

  updateByYear(year: string, formData: FormData): Observable<Calendar> {
    // NOTE: this request is specifically *not* sending a Content-Type HTTP request header
    // because Angular needs to form that value on it's own when sending a request of
    // Content-Type multipart/form-data
    const url = this.apiSchoolCalendarUrl + `/${year.replace(/\s+/g, '')}`;
    return this.httpClient.put<any>(url, formData);
  }
}
