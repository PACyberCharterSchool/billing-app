import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';

import { Globals } from '../globals';

import { environment } from '../../environments/environment';

import { StudentRecordsHeader, StudentRecord } from '../models/student-record.model';

@Injectable()
export class StudentRecordsService {
  private apiSSRUrl;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private globals: Globals, private httpClient: HttpClient) {
    this.apiSSRUrl = 'api/StudentRecords';
  }

  public postLockStudentData(scope: string): Observable<any> {
    const url = this.apiSSRUrl + `/header/${scope}/lock`;
    return this.httpClient.post<any>(url, this.headers);
  }

  public getHeaders(locked: boolean): Observable<StudentRecordsHeader[]> {
    const url = this.apiSSRUrl + `/scopes` + (locked !== null ? `?Locked=${locked}` : ``);
    return this.httpClient.get<StudentRecordsHeader[]>(url, this.headers);
    // return Observable.of(studentRecordsHeaders);
  }

  public getHeaderByScope(scope: string, skip: number): Observable<StudentRecordsHeader> {
    const url = this.apiSSRUrl + `/header/${scope}?Skip=${skip}&Take=${this.globals.take}`;
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }

  public getHeaderByScopeByDob(scope: string, date: Date): Observable<StudentRecordsHeader> {
    const url = this.buildStudentDOBQuery(scope, date);
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }

  private buildStudentDOBQuery(scope: string, dob: Date): string {
    const url: string = this.apiSSRUrl + `/header/${scope}?Filter=(StudentDateOfBirth eq ${dob.toLocaleDateString('en-US')})`;
    return url;
  }

  public getHeaderByScopeByIep(scope: string, isIep: boolean): Observable<StudentRecordsHeader> {
    const url: string = this.buildStudentIepSearchQuery(scope, isIep);
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }

  private buildStudentIepSearchQuery(scope: string, isIep: boolean): string {
    const url: string = this.apiSSRUrl + `/header/${scope}?Filter=(StudentIsSpecialEducation eq ${isIep})`;
    return url;
  }

  public getHeaderByScopeByStart(scope: string, start: Date): Observable<StudentRecordsHeader> {
    const url: string = this.buildStudentsStartDateQuery(scope, start);
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }

  private buildStudentsStartDateQuery(scope: string, start: Date): string {
    const url: string = this.apiSSRUrl + `/header/${scope}?Filter=(StudentEnrollmentDate eq ${start.toLocaleDateString('en-US')})`;
    return url;
  }

  public getHeaderByScopeByStartByEnd(scope: string, start: Date, end: Date): Observable<StudentRecordsHeader> {
    const url: string = this.buildStudentsStartDateEndDateQuery(scope, start, end);
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }

  private buildStudentsStartDateEndDateQuery(scope: string, start: Date, end: Date): string {
    const url: string = this.apiSSRUrl +
      `/header/${scope}?Filter=` +
      `(((StudentEnrollmentDate ge ${start.toLocaleDateString('en-US')}) and ` +
      `(StudentEnrollmentDate le ${end.toLocaleDateString('en-US')})) and ` +
      `((StudentWithdrawalDate ge ${end.toLocaleDateString('en-US')}) and ` +
      `(StudentWithdrawalDate le ${start.toLocaleDateString('en-US')})))`;
    return url;
  }

  public getHeaderByScopeByEnd(scope: string, end: Date): Observable<StudentRecordsHeader> {
    const url: string = this.buildStudentsEndDateQuery(scope, end);
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }

  private buildStudentsEndDateQuery(scope: string, end: Date): string {
    const url: string = this.apiSSRUrl + `/header/${scope}?Filter=(StudentWithdrawalDate eq ${end.toLocaleDateString('en-US')})`;
    return url;
  }

  public getHeaderByScopeByGrade(scope: string, grade: number): Observable<StudentRecordsHeader> {
    const url: string = this.buildStudentsGradeQuery(scope, grade);
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }

  private buildStudentsGradeQuery(scope: string, grade: number): string {
    const url: string = this.apiSSRUrl + `/header/${scope}?Filter=(StudentGradeLevel eq ${grade})`;
    return url;
  }

  public getHeaderByScopeBySchoolDistrict(scope: string, schoolDistrictId: number): Observable<StudentRecordsHeader> {
    const url: string = this.buildStudentsSchoolDistrictQuery(scope, schoolDistrictId);
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }

  private buildStudentsSchoolDistrictQuery(scope: string, schoolDistrictId: number): string {
    const url: string = this.apiSSRUrl + `/header/${scope}?Filter=(SchoolDistrictId eq ${schoolDistrictId})`;
    return url;
  }

  public updateStudentRecord(scope: string, record: StudentRecord): Observable<StudentRecord> {
    const url = this.apiSSRUrl + `/${scope}/${record.id}`;

    delete record.id;
    delete record.header;
    delete record.lazyLoader;
    delete record.lastUpdated;

    return this.httpClient.put<StudentRecord>(url, record, this.headers);
  }

  public getHeaderByScopeByNameById(scope: string, search: string): Observable<StudentRecordsHeader> {
    const url: string = this.buildStudentsNameAndIdQuery(scope, search);
    return this.httpClient.get<StudentRecordsHeader>(url, this.headers);
  }

  private buildStudentsNameAndIdQuery(scope: string, search: string): string {
    let query: string;
    if (Number(search)) {
      query = `(StudentId eq ${search})`;
    } else {
      query = `((StudentFirstName has ${search}) or (StudentLastName has ${search}))`;
    }
    const url: string = this.apiSSRUrl + `/header/${scope}?Filter=${query}`;
    return url;
  }
}
