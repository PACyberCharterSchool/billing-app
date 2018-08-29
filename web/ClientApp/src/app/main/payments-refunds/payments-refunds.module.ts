import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SharedModule } from '../../shared/shared.module';

import { PaymentsRefundsRoutingModule } from './payments-refunds-routing.module';

import { PaymentsRefundsHomeComponent } from './payments-refunds-home/payments-refunds-home.component';
import { RefundsComponent } from './refunds/refunds.component';
import { RefundsListComponent } from './refunds-list/refunds-list.component';
import { RefundUpsertFormComponent } from './refund-upsert-form/refund-upsert-form.component';

import { PaymentsComponent } from './payments/payments.component';
import { PaymentsListComponent } from './payments-list/payments-list.component';
import { PaymentUpsertFormComponent } from './payment-upsert-form/payment-upsert-form.component';

import { PaymentsRefundsComponent } from './payments-refunds.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { NgxCurrencyModule } from 'ngx-currency';

@NgModule({
  declarations: [
    PaymentsRefundsComponent,
    PaymentsRefundsHomeComponent,
    PaymentsComponent,
    RefundsComponent,
    RefundsListComponent,
    RefundUpsertFormComponent,
    PaymentsListComponent,
    PaymentUpsertFormComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    PaymentsRefundsRoutingModule,
    SharedModule,
    NgbModule,
    NgxCurrencyModule
  ],
  providers: [ ],
  entryComponents: [ PaymentUpsertFormComponent, RefundUpsertFormComponent ]
})

export class PaymentsRefundsModule { }
