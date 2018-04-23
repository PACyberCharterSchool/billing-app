import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ActionContentRoutingModule } from './action-content-routing.module';

import { StudentsComponent } from './students/students.component';
import { ActionContentComponent } from './action-content.component';
import { ActionContentHomeComponent } from './action-content-home/action-content-home.component';

@NgModule({
  declarations: [
    StudentsComponent,
    ActionContentComponent,
    ActionContentHomeComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ActionContentRoutingModule
  ],
  providers: []
})

export class ActionContentModule { }
