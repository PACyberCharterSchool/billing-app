import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainComponent } from '../main.component';

import { AdministrationHomeComponent } from './administration-home/administration-home.component';

const adminRoutes: Routes = [
  {
    path: 'students',
    component: MainComponent,
    children: [
      {
        path: 'list',
        component: AdministrationHomeComponent,
        outlet: 'action'
      }
    ]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(adminRoutes)
  ],
  exports: [
    RouterModule
  ],
  declarations: [],
  providers: []
})

export class AdministrationRoutingModule { }
