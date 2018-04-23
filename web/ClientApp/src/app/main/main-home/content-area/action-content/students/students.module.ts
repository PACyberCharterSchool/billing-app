import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { StudentsRoutingModule } from './students-routing.module';

import { StudentsComponent } from './students.component';
import { StudentsListComponent } from './students-list/students-list.component';
import { StudentsDetailComponent } from './students-detail/students-detail.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    StudentsRoutingModule
  ],
  declarations: [
    StudentsComponent,
    StudentsListComponent,
    StudentsDetailComponent
  ],
  providers: []
})

export class StudentsModule { }
