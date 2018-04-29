import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ReportsInvoicesRoutingModule } from './reports-invoices-routing.module';

import { ReportsInvoicesComponent } from './reports-invoices.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    ReportsInvoicesComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReportsInvoicesRoutingModule,
    NgbModule
  ],
  providers: [ ]
})

export class ReportsInvoicesModule { }
