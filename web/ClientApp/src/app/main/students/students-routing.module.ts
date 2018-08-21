import { NgModule } from '@angular/core';
import { RouterModule, Routes, RouterOutlet } from '@angular/router';

import { StudentsComponent } from './students.component';
import { StudentsListComponent } from './students-list/students-list.component';
import { StudentsDetailComponent } from './students-detail/students-detail.component';
import { MainComponent } from '../main.component';

import { AuthenticationGuardService } from '../../services/authentication-guard.service';

const studentsRoutes: Routes = [
  {
    path: 'students',
    component: MainComponent,
    children: [
      {
        path: 'list',
        component: StudentsListComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: ':scope/:id',
        component: StudentsDetailComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: ':id',
        component: StudentsDetailComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
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
