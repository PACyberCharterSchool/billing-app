import { NgModule } from '@angular/core';
import { RouterModule, Routes, RouterOutlet } from '@angular/router';

import { MainComponent } from '../main.component';

import { AdministrationHomeComponent } from './administration-home/administration-home.component';
import { AdministrationPaymentRateComponent } from './administration-payment-rate/administration-payment-rate.component';

const adminRoutes: Routes = [
  {
    path: 'administration',
    component: MainComponent,
    children: [
      {
        path: 'payment-rates',
        component: AdministrationPaymentRateComponent,
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
