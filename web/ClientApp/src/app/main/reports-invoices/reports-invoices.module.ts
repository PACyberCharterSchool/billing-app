import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ReportsInvoicesRoutingModule } from './reports-invoices-routing.module';

import { ReportsInvoicesComponent } from './reports-invoices.component';
import { ReportsInvoicesHomeComponent } from './reports-invoices-home/reports-invoices-home.component';
import { InvoicesDigitalSignaturesListComponent } from './invoices-digital-signatures-list/invoices-digital-signatures-list.component';
import { DigitalSignatureUpsertFormComponent } from './digital-signature-upsert-form/digital-signature-upsert-form.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [
    ReportsInvoicesComponent,
    ReportsInvoicesHomeComponent,
    InvoicesDigitalSignaturesListComponent,
    DigitalSignatureUpsertFormComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReportsInvoicesRoutingModule,
    NgbModule,
    SharedModule
  ],
  providers: [ ],
  entryComponents: [ DigitalSignatureUpsertFormComponent ]
})

export class ReportsInvoicesModule { }
