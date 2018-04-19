import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { ActionContentRoutingModule } from './action-content-routing.module';

import { StudentsComponent } from './students/students.component';

@NgModule({
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ActionContentRoutingModule
  ],
  declarations: [
    StudentsComponent
  ],
  providers: []
})

export class ActionContentModule { }
