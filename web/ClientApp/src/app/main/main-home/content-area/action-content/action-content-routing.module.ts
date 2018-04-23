import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ActionContentComponent } from './action-content.component';
import { ActionContentHomeComponent } from './action-content-home/action-content-home.component';
import { StudentsComponent } from './students/students.component';

const actionContentRoutes: Routes = [
  {
    path: '',
    component: ActionContentComponent,
    children: [
      {
        path: '',
        component: ActionContentHomeComponent,
        children: [
          {
            path: 'students',
            component: StudentsComponent
          }
        ]
      }
    ]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(actionContentRoutes)
  ],
  exports: [
    RouterModule
  ],
  declarations: [],
  providers: []
})

export class ActionContentRoutingModule { }
