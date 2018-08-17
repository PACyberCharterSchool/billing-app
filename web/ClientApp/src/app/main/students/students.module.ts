import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { SharedModule } from '../../shared/shared.module';

import { StudentsRoutingModule } from './students-routing.module';

import { StudentsComponent } from './students.component';
import { StudentsListComponent } from './students-list/students-list.component';
import { StudentsDetailComponent } from './students-detail/students-detail.component';
import { StudentDatepickerComponent } from './student-datepicker/student-datepicker.component';
import { StudentDetailsInfoComponent } from './student-details-info/student-details-info.component';
import { StudentHistoryInfoComponent } from './student-history-info/student-history-info.component';
import { StudentActivityHistoryComponent } from './student-activity-history/student-activity-history.component';
import { StudentAdvancedFilterComponent } from './student-advanced-filter/student-advanced-filter.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerModule } from 'ngx-spinner';

import { IepEnrolledPipe } from '../../pipes/iep-enrolled.pipe';

@NgModule({
  declarations: [
    StudentsComponent,
    StudentsListComponent,
    StudentsDetailComponent,
    StudentDatepickerComponent,
    StudentDetailsInfoComponent,
    StudentHistoryInfoComponent,
    StudentActivityHistoryComponent,
    StudentAdvancedFilterComponent,
    IepEnrolledPipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    StudentsRoutingModule,
    NgbModule,
    NgxSpinnerModule,
    HttpClientModule,
    SharedModule
  ],
  providers: [ ]
})

export class StudentsModule { }
