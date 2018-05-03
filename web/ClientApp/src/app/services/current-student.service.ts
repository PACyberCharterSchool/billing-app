import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

import { Student } from '../models/student.model';

@Injectable()
export class CurrentStudentService {

  private studentSource = new BehaviorSubject<Student>(new Student());
  currentStudent = this.studentSource.asObservable();

  constructor() { }

  changeStudent(student: Student) {
    this.studentSource.next(student);
  }
}
