import { NgModule } from '@angular/core';
import { RouterModule, Routes, RouterOutlet } from '@angular/router';

import { MainComponent } from '../main.component';

import { ReportsInvoicesHomeComponent } from './reports-invoices-home/reports-invoices-home.component';
import { ReportsInvoicesComponent } from './reports-invoices.component';

const reportsInvoicesRoutes: Routes = [
  {
    path: 'reports-invoices',
    component: MainComponent,
    children: [
      {
        path: '',
        component: ReportsInvoicesComponent,
        outlet: 'action'
      },
      {
        path: 'home',
        component: ReportsInvoicesHomeComponent,
        outlet: 'action'
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
