import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { StudentsComponent } from './students.component';
import { StudentsListComponent } from './students-list/students-list.component';

const studentsRoutes: Routes = [
  {
    path: 'students',
    component: StudentsComponent,
    children: [
      {
        path: '',
        component: StudentsListComponent
      }
    ]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(studentsRoutes)
  ],
  exports: [
    RouterModule
  ],
  declarations: [],
  providers: []
})

export class StudentsRoutingModule { }
