import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ActionContentComponent } from './action-content.component';
// import { StudentsComponent } from './students/students.component';

const actionContentRoutes: Routes = [
  {
    path: '',
    component: ActionContentComponent,
    // children: [
    //   {
    //     path: 'students',
    //     component: StudentsComponent
    //   }
    // ]
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
