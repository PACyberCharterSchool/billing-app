import { NgModule } from '@angular/core';
import { RouterModule, Routes, RouterOutlet } from '@angular/router';

import { MainComponent } from '../main.component';

import { ReportsInvoicesHomeComponent } from './reports-invoices-home/reports-invoices-home.component';
import { ReportsInvoicesComponent } from './reports-invoices.component';
import { InvoicesDigitalSignaturesListComponent } from './invoices-digital-signatures-list/invoices-digital-signatures-list.component';
import { InvoicesListComponent } from './invoices-list/invoices-list.component';
import { InvoicesMonthlyCombinedListComponent } from './invoices-monthly-combined-list/invoices-monthly-combined-list.component';
import { StudentActivityListComponent } from './student-activity-list/student-activity-list.component';
import { AccountsReceivableAsOfComponent } from './accounts-receivable-as-of/accounts-receivable-as-of.component';
import { CsiuListComponent } from './csiu-list/csiu-list.component';

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
      },
      {
        path: 'digital-signatures',
        component: InvoicesDigitalSignaturesListComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'invoices',
        component: InvoicesListComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'invoices-monthly-combined',
        component: InvoicesMonthlyCombinedListComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'itemized-student-activity',
        component: StudentActivityListComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'as-of-date-reports',
        component: AccountsReceivableAsOfComponent,
        outlet: 'action',
        canActivate: [ AuthenticationGuardService ]
      },
      {
        path: 'csiu-reports',
        component: CsiuListComponent,
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
