import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InfiniteScrollModule } from 'ngx-infinite-scroll';

import { NormalizeFieldNamePipe } from '../pipes/normalize-field-name.pipe';
import { OrderByPipe } from '../pipes/orderby.pipe';
import { NormalizeFieldValuePipe } from '../pipes/normalize-field-value.pipe';
import { InterpretAuditTypePipe } from '../pipes/interpret-audit-type.pipe';

@NgModule({
  declarations: [
    NormalizeFieldNamePipe,
    OrderByPipe,
    NormalizeFieldValuePipe,
    InterpretAuditTypePipe
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
    InfiniteScrollModule
  ],
  providers: []
})

export class SharedModule {}
