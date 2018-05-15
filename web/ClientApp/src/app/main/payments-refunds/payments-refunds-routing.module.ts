import { NgModule } from '@angular/core';
import { RouterModule, Routes, RouterOutlet } from '@angular/router';

import { MainComponent } from '../main.component';

import { PaymentsRefundsHomeComponent } from './payments-refunds-home/payments-refunds-home.component';
import { PaymentsComponent } from './payments/payments.component';
import { RefundsComponent } from './refunds/refunds.component';
import { PaymentsListComponent } from './payments-list/payments-list.component';

import { AuthenticationGuardService } from '../../services/authentication-guard.service';

const paymentsRefundsRoutes: Routes = [
  {
    path: 'payments-refunds',
    component: MainComponent,
    canActivate: [ AuthenticationGuardService ],
    children: [
      {
        path: 'home',
        component: PaymentsRefundsHomeComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'payments',
        component: PaymentsComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'refunds',
        component: RefundsComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'list',
        component: PaymentsListComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      }
    ]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(paymentsRefundsRoutes)
  ],
  exports: [
    RouterModule
  ],
  declarations: [],
  providers: []
})

export class PaymentsRefundsRoutingModule { }
