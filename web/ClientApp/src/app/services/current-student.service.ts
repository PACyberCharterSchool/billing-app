import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

import { StudentRecord } from '../models/student-record.model';

@Injectable()
export class CurrentStudentService {

  private studentSource = new BehaviorSubject<StudentRecord>(new StudentRecord());
  currentStudent = this.studentSource.asObservable();

  constructor() { }

  changeStudent(student: StudentRecord) {
    this.studentSource.next(student);
  }
}
