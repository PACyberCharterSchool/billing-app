import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NormalizeFieldNamePipe } from '../pipes/normalize-field-name.pipe';
import { OrderByPipe } from '../pipes/orderby.pipe';

@NgModule({
  declarations: [
    NormalizeFieldNamePipe,
    OrderByPipe
  ],
  imports: [
    CommonModule
  ],
  exports: [
    NormalizeFieldNamePipe,
    OrderByPipe
  ],
  providers: []
})

export class SharedModule {}
