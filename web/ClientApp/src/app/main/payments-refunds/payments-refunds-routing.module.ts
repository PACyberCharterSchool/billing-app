import { NgModule } from '@angular/core';
import { RouterModule, Routes, RouterOutlet } from '@angular/router';

import { MainComponent } from '../main.component';

import { PaymentsRefundsHomeComponent } from './payments-refunds-home/payments-refunds-home.component';

const paymentsRefundsRoutes: Routes = [
  {
    path: 'payments-refunds',
    component: MainComponent,
    children: [
      {
        path: 'home',
        component: PaymentsRefundsHomeComponent,
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
