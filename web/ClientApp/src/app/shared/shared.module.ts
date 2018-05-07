import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InfiniteScrollModule } from 'ngx-infinite-scroll';

import { NormalizeFieldNamePipe } from '../pipes/normalize-field-name.pipe';
import { OrderByPipe } from '../pipes/orderby.pipe';

@NgModule({
  declarations: [
    NormalizeFieldNamePipe,
    OrderByPipe
  ],
  imports: [
    CommonModule,
    InfiniteScrollModule
  ],
  exports: [
    NormalizeFieldNamePipe,
    OrderByPipe,
    InfiniteScrollModule
  ],
  providers: []
})

export class SharedModule {}
