import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AdministrationRoutingModule } from './administration-routing.module';

import { AdministrationPaymentRateComponent } from './administration-payment-rate/administration-payment-rate.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    AdministrationPaymentRateComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    AdministrationRoutingModule,
    NgbModule
  ],
  providers: [ ]
})

export class AdministrationModule { }
