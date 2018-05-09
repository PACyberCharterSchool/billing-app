import { NgModule } from '@angular/core';
import { RouterModule, Routes, RouterOutlet } from '@angular/router';

import { MainComponent } from '../main.component';

import { PaymentsRefundsHomeComponent } from './payments-refunds-home/payments-refunds-home.component';
import { PaymentsComponent } from './payments/payments.component';
import { RefundsComponent } from './refunds/refunds.component';
import { PaymentsListComponent } from './payments-list/payments-list.component';

const paymentsRefundsRoutes: Routes = [
  {
    path: 'payments-refunds',
    component: MainComponent,
    children: [
      {
        path: 'home',
        component: PaymentsRefundsHomeComponent,
        outlet: 'action'
      },
      {
        path: 'payments',
        component: PaymentsComponent,
        outlet: 'action'
      },
      {
        path: 'refunds',
        component: RefundsComponent,
        outlet: 'action'
      },
      {
        path: 'list',
        component: PaymentsListComponent,
        outlet: 'action'
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
