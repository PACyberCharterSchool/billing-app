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

  getByYear(year: string): Observable<Calendar> {
    const url = this.apiSchoolCalendarUrl + `/${year}`;
    return this.httpClient.get<Calendar>(url, this.headers);
  }

  updateByYear(year: string, file: any): Observable<Calendar> {
    const url = this.apiSchoolCalendarUrl + `/${year}`;
    return this.httpClient.put<any>(url, file, this.headers);
  }
}
