import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { ContentAreaRoutingModule } from './content-area-routing.module';

@NgModule({
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ContentAreaRoutingModule
  ],
  declarations: [],
  providers: []
})

export class ContentAreaModule { }
