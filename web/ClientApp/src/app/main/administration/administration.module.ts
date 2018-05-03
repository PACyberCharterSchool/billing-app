import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AdministrationRoutingModule } from './administration-routing.module';

import { AdministrationHomeComponent } from './administration-home/administration-home.component';
import { AdministrationPaymentRateComponent } from './administration-payment-rate/administration-payment-rate.component';
import {
  AdministrationImportStudentDataComponent
} from './administration-import-student-data/administration-import-student-data.component';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { NormalizeFieldNamePipe } from '../../pipes/normalize-field-name.pipe';

@NgModule({
  declarations: [
    AdministrationPaymentRateComponent,
    AdministrationHomeComponent,
    AdministrationImportStudentDataComponent,
    NormalizeFieldNamePipe
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
