import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { StudentsComponent } from './students.component';
import { StudentsListComponent } from './students-list/students-list.component';
import { StudentsDetailComponent } from './students-detail/students-detail.component';
import { MainComponent } from '../main.component';

const studentsRoutes: Routes = [
  {
    path: 'students',
    component: MainComponent,
    children: [
      {
        path: 'list',
        component: StudentsListComponent,
        outlet: 'action'
      },
      {
        path: ':id',
        component: StudentsDetailComponent,
        outlet: 'action'
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
