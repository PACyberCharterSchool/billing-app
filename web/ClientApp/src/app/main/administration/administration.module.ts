import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SharedModule } from '../../shared/shared.module';

import { AdministrationRoutingModule } from './administration-routing.module';

import { AdministrationHomeComponent } from './administration-home/administration-home.component';
import { AdministrationPaymentRateListComponent } from './administration-payment-rate-list/administration-payment-rate-list.component';
import {
  AdministrationImportStudentDataComponent
} from './administration-import-student-data/administration-import-student-data.component';
import { AdministrationPaymentRateUpdateFormComponent } from './administration-payment-rate-update-form/administration-payment-rate-update-form.component';
import { AdministrationSchoolCalendarComponent } from './administration-school-calendar/administration-school-calendar.component';
import { AdministrationAuditListComponent } from './administration-audit-list/administration-audit-list.component';

import { NgbModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerModule } from 'ngx-spinner';

@NgModule({
  declarations: [
    AdministrationPaymentRateListComponent,
    AdministrationHomeComponent,
    AdministrationImportStudentDataComponent,
    AdministrationPaymentRateUpdateFormComponent,
    AdministrationSchoolCalendarComponent,
    AdministrationAuditListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    AdministrationRoutingModule,
    SharedModule,
    NgbModule,
    NgxSpinnerModule
  ],
  providers: [ NgbActiveModal ],
  entryComponents: [ AdministrationPaymentRateUpdateFormComponent ]
})

export class AdministrationModule { }
