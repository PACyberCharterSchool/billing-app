import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { StudentsService } from '../../../services/students.service';

import { Student } from '../../../models/student.model';

@Component({
  selector: 'app-students-detail',
  templateUrl: './students-detail.component.html',
  styleUrls: ['./students-detail.component.scss']
})
export class StudentsDetailComponent implements OnInit, OnDestroy {

  private student: Student;
  private subscription: any;

  constructor(private studentsService: StudentsService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.subscription = this.route.params.subscribe(
      params => {
        console.log('StudentsDetailComponent.ngOnInit(): student id is ', params['id']);
        this.studentsService.getStudent(+params['id']).subscribe(
          data => {
            console.log('StudentsDetailComponent.ngOnInit(): data is ', data);
            this.student = data['student'];
         }
       );
      }
    );
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
