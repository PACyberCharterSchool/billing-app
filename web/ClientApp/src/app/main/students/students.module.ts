import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { StudentsRoutingModule } from './students-routing.module';

import { StudentsComponent } from './students.component';
import { StudentsListComponent } from './students-list/students-list.component';
import { StudentsDetailComponent } from './students-detail/students-detail.component';
import { StudentDatepickerComponent } from './student-datepicker/student-datepicker.component';
import { StudentDetailsInfoComponent } from './student-details-info/student-details-info.component';
import { StudentHistoryInfoComponent } from './student-history-info/student-history-info.component';
import { StudentAdvancedFilterComponent } from './student-advanced-filter/student-advanced-filter.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { IepEnrolledPipe } from '../../pipes/iep-enrolled.pipe';
import { OrderByPipe } from '../../pipes/orderby.pipe';

@NgModule({
  declarations: [
    StudentsComponent,
    StudentsListComponent,
    StudentsDetailComponent,
    StudentDatepickerComponent,
    StudentDetailsInfoComponent,
    StudentHistoryInfoComponent,
    StudentAdvancedFilterComponent,
    IepEnrolledPipe,
    OrderByPipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    StudentsRoutingModule,
    NgbModule
  ],
  providers: [ ]
})

export class StudentsModule { }
