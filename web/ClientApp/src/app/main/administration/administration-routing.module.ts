import { NgModule } from '@angular/core';
import { RouterModule, Routes, RouterOutlet } from '@angular/router';

import { MainComponent } from '../main.component';

import { AdministrationHomeComponent } from './administration-home/administration-home.component';
import { AdministrationPaymentRateComponent } from './administration-payment-rate/administration-payment-rate.component';
import {
  AdministrationImportStudentDataComponent
} from './administration-import-student-data/administration-import-student-data.component';

const adminRoutes: Routes = [
  {
    path: 'administration',
    component: MainComponent,
    children: [
      {
        path: 'home',
        component: AdministrationHomeComponent,
        outlet: 'action'
      },
      {
        path: 'payment-rates',
        component: AdministrationPaymentRateComponent,
        outlet: 'action'
      },
      {
        path: 'import-student-data',
        component: AdministrationImportStudentDataComponent,
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
