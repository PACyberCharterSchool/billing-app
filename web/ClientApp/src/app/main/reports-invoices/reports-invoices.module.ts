import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ReportsInvoicesRoutingModule } from './reports-invoices-routing.module';

import { ReportsInvoicesComponent } from './reports-invoices.component';
import { ReportsInvoicesHomeComponent } from './reports-invoices-home/reports-invoices-home.component';
import { InvoicesDigitalSignaturesListComponent } from './invoices-digital-signatures-list/invoices-digital-signatures-list.component';
import { DigitalSignatureUpsertFormComponent } from './digital-signature-upsert-form/digital-signature-upsert-form.component';
import { InvoicesListComponent } from './invoices-list/invoices-list.component';
import { InvoiceCreateFormComponent } from './invoice-create-form/invoice-create-form.component';
import { InvoicePreviewFormComponent } from './invoice-preview-form/invoice-preview-form.component';
import { InvoiceExcelPreviewComponent } from './invoice-excel-preview/invoice-excel-preview.component';

import { ExcelComponent } from './excel/excel.component';
import { CellComponent } from './cell/cell.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HotTableModule, HotTableRegisterer } from '@handsontable/angular';

import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [
    ReportsInvoicesComponent,
    ReportsInvoicesHomeComponent,
    InvoicesDigitalSignaturesListComponent,
    DigitalSignatureUpsertFormComponent,
    InvoicesListComponent,
    InvoiceCreateFormComponent,
    InvoicePreviewFormComponent,
    InvoiceExcelPreviewComponent,
    ExcelComponent,
    CellComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ReportsInvoicesRoutingModule,
    NgbModule,
    HotTableModule,
    SharedModule
  ],
  providers: [ HotTableRegisterer ],
  entryComponents: [
    DigitalSignatureUpsertFormComponent,
    InvoiceCreateFormComponent,
    InvoicePreviewFormComponent
  ]
})

export class ReportsInvoicesModule { }
