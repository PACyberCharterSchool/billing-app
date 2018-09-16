import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InfiniteScrollModule } from 'ngx-infinite-scroll';

import { NormalizeFieldNamePipe } from '../pipes/normalize-field-name.pipe';
import { OrderByPipe } from '../pipes/orderby.pipe';
import { NormalizeFieldValuePipe } from '../pipes/normalize-field-value.pipe';
import { InterpretAuditTypePipe } from '../pipes/interpret-audit-type.pipe';
import { AcademicYearConflictValidatorDirective } from './academic-year-conflict-validator.directive';
import { EnrollmentWithdrawalDateConflictValidatorDirective } from './enrollment-withdrawal-date-conflict-validator.directive';
import { FormerCurrentIepDateConflictValidatorDirective } from './former-current-iep-date-conflict-validator.directive';

@NgModule({
  declarations: [
    NormalizeFieldNamePipe,
    OrderByPipe,
    NormalizeFieldValuePipe,
    InterpretAuditTypePipe,
    AcademicYearConflictValidatorDirective,
    EnrollmentWithdrawalDateConflictValidatorDirective,
    FormerCurrentIepDateConflictValidatorDirective
  ],
  imports: [
    CommonModule,
    InfiniteScrollModule
  ],
  exports: [
    NormalizeFieldNamePipe,
    NormalizeFieldValuePipe,
    OrderByPipe,
    InterpretAuditTypePipe,
    InfiniteScrollModule,
    AcademicYearConflictValidatorDirective,
    EnrollmentWithdrawalDateConflictValidatorDirective,
    FormerCurrentIepDateConflictValidatorDirective
  ],
  providers: []
})

export class SharedModule {}
