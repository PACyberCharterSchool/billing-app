import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SharedModule } from '../../shared/shared.module';

import { AdministrationRoutingModule } from './administration-routing.module';

import { AdministrationHomeComponent } from './administration-home/administration-home.component';
import { AdministrationPaymentRateComponent } from './administration-payment-rate/administration-payment-rate.component';
import {
  AdministrationImportStudentDataComponent
} from './administration-import-student-data/administration-import-student-data.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    AdministrationPaymentRateComponent,
    AdministrationHomeComponent,
    AdministrationImportStudentDataComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    AdministrationRoutingModule,
    SharedModule,
    NgbModule
  ],
  providers: [ ]
})

export class AdministrationModule { }
