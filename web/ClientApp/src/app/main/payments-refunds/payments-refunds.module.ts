import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SharedModule } from '../../shared/shared.module';

import { PaymentsRefundsRoutingModule } from './payments-refunds-routing.module';

import { PaymentsRefundsHomeComponent } from './payments-refunds-home/payments-refunds-home.component';
import { PaymentsComponent } from './payments/payments.component';
import { RefundsComponent } from './refunds/refunds.component';
import { PaymentsListComponent } from './payments-list/payments-list.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    PaymentsRefundsHomeComponent,
    PaymentsComponent,
    RefundsComponent,
    PaymentsListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    PaymentsRefundsRoutingModule,
    SharedModule,
    NgbModule
  ],
  providers: [ ]
})

export class PaymentsRefundsModule { }
