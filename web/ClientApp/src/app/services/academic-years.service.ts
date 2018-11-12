import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient, HttpHeaders } from '@angular/common/http';

const baseYear = '2000';

class YearsResponse {
  years: string[];
}

@Injectable()
export class AcademicYearsService {
  private years: string[];

  constructor(private http: HttpClient) {
    this.http.get<YearsResponse>('/api/calendars/years', {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    }).subscribe(res => this.years = res.years);
  }

  getAcademicYears(): string[] {
    return this.years;
  }
}
