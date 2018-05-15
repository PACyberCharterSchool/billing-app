import { NgModule } from '@angular/core';
import { RouterModule, Routes, RouterOutlet } from '@angular/router';

import { MainComponent } from '../main.component';

import { ReportsInvoicesHomeComponent } from './reports-invoices-home/reports-invoices-home.component';
import { ReportsInvoicesComponent } from './reports-invoices.component';

import { AuthenticationGuardService } from '../../services/authentication-guard.service';

const reportsInvoicesRoutes: Routes = [
  {
    path: 'reports-invoices',
    component: MainComponent,
    canActivate: [ AuthenticationGuardService ],
    children: [
      {
        path: '',
        component: ReportsInvoicesComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'home',
        component: ReportsInvoicesHomeComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      }
    ]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(reportsInvoicesRoutes)
  ],
  exports: [
    RouterModule
  ],
  declarations: [],
  providers: []
})

export class ReportsInvoicesRoutingModule { }
