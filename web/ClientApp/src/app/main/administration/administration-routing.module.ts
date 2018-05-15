import { NgModule } from '@angular/core';
import { RouterModule, Routes, RouterOutlet } from '@angular/router';

import { MainComponent } from '../main.component';

import { AdministrationHomeComponent } from './administration-home/administration-home.component';
import { AdministrationPaymentRateComponent } from './administration-payment-rate/administration-payment-rate.component';
import {
  AdministrationImportStudentDataComponent
} from './administration-import-student-data/administration-import-student-data.component';

import { AuthenticationGuardService } from '../../services/authentication-guard.service';

const adminRoutes: Routes = [
  {
    path: 'administration',
    component: MainComponent,
    canActivate: [ AuthenticationGuardService ],
    children: [
      {
        path: 'home',
        component: AdministrationHomeComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'payment-rates',
        component: AdministrationPaymentRateComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'import-student-data',
        component: AdministrationImportStudentDataComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
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
