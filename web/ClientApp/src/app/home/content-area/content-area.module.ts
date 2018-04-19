import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { ContentAreaRoutingModule } from './content-area-routing.module';

import { ActionContentModule } from './action-content/action-content.module';

@NgModule({
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ActionContentModule,
    ContentAreaRoutingModule
  ],
  declarations: [],
  providers: []
})

export class ContentAreaModule { }
