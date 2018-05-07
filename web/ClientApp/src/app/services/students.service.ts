import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { environment } from '../../environments/environment';

import { Student } from '../models/student.model';
import { SchoolDistrict } from '../models/school-district.model';
import { StudentActivityRecord } from '../models/student-activity-record.model';

import { Globals } from '../globals';

@Injectable()
export class StudentsService {
  private apiStudentsUrl: string;
  private headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private globals: Globals, private httpClient: HttpClient) {
    this.apiStudentsUrl = 'http://localhost:5000/api/students';
  }

  public getStudents(skip: number): Observable<Student[]> {
    const url = this.apiStudentsUrl + `?skip=${skip}&take=${this.globals.take}`;
    return this.httpClient.get<Student[]>(this.apiStudentsUrl, this.headers);
  }

  public getStudent(id: number): Observable<Student> {
    return this.httpClient.get<Student>(this.apiStudentsUrl + `/${id}`, this.headers);
  }

  public getStudentsFilteredByIep(isIep: boolean): Observable<Student[]> {
    const url: string = this.buildStudentIepSearchQuery(isIep);
    return this.httpClient.get<Student[]>(url, this.headers);
  }

  public getStudentsFilteredByNameOrId(searchText: string): Observable<Student[]> {
    const url: string = this.buildStudentIdOrNameSearchQuery(searchText);
    return this.httpClient.get<Student[]>(url, this.headers);
  }

  public getStudentsFilteredBySchoolDistrict(schoolId: number): Observable<Student[]> {
    const url: string = this.buildStudentSchoolDistrictSearchQuery(schoolId);
    return this.httpClient.get<Student[]>(url, this.headers);
  }

  public getStudentsFilteredByGrade(grade: number): Observable<Student[]> {
    const url: string = this.buildStudentGradeSearchQuery(grade);
    return this.httpClient.get<Student[]>(url, this.headers);
  }

  public getStudentsFilteredByDateOfBirth(dob: Date): Observable<Student[]> {
    const url: string = this.buildStudentDateOfBirthSearchQuery(dob);
    return this.httpClient.get<Student[]>(url, this.headers);
  }

  public getStudentsFilteredByStartDate(startDate: Date): Observable<Student[]> {
    const url: string = this.buildStudentStartDateSearchQuery(startDate);
    return this.httpClient.get<Student[]>(url, this.headers);
  }

  public getStudentsFilteredByEndDate(endDate: Date): Observable<Student[]> {
    const url: string = this.buildStudentEndDateSearchQuery(endDate);
    return this.httpClient.get<Student[]>(url, this.headers);
  }

  public getStudentActivityRecordsByStudentId(id: number) {
    const url: string = `http://localhost:5000/api/StudentActivityRecords/${id}`;
    return this.httpClient.get<StudentActivityRecord[]>(url, this.headers);
  }

  private buildStudentIepSearchQuery(isIep: boolean): string {
    const url: string = this.apiStudentsUrl + `?filter=(isSpecialEducation eq ${isIep})`;
    return url;
  }

  private buildStudentStartDateSearchQuery(startDate: Date): string {
    const url: string = this.apiStudentsUrl + `?filter=(StartDate eq ${startDate.toLocaleDateString('en-US')})`;
    return url;
  }

  private buildStudentEndDateSearchQuery(endDate: Date): string {
    const url: string = this.apiStudentsUrl + `?filter=(EndDate eq ${endDate.toLocaleDateString('en-US')})`;
    return url;
  }

  private buildStudentGradeSearchQuery(grade: number): string {
    const url: string = this.apiStudentsUrl + `?filter=(Grade eq ${grade})`;
    return url;
  }

  private buildStudentSchoolDistrictSearchQuery(schoolId: number): string {
    const url: string = this.apiStudentsUrl + `?filter=(schoolDistrict.id eq ${schoolId})`;
    return url;
  }

  private buildStudentDateOfBirthSearchQuery(dob: Date): string {
    const url: string = this.apiStudentsUrl + `?filter=(dateOfBirth eq ${dob.toLocaleDateString('en-US')})`;
    return url;
  }

  private buildStudentIdOrNameSearchQuery(searchText: string): string {
    let url: string;
    let searchTokens: string[];
    let newSearchTokens: string[];

    if (searchText) {
      searchTokens = searchText.split(' ');
      newSearchTokens = searchTokens.map(
        (v) => {
          if (isNaN(Number(v))) {
            return `(FirstName eq ${v}) or (LastName eq ${v})`;
          } else {
            return `(PACyberId eq ${v}) or (PASecuredId eq ${v})`;
          }
        }
      );

      url = this.apiStudentsUrl + '?filter=' + '(' + newSearchTokens.join(' ') + ')';
      console.log('StudentsService.buildStudentIdOrNameSearchQuery(): url is ', url);
    }

    return url;
  }
}
