import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AdministrationRoutingModule } from './administration-routing.module';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { IepEnrolledPipe } from '../../pipes/iep-enrolled.pipe';
import { OrderByPipe } from '../../pipes/orderby.pipe';

@NgModule({
  declarations: [
    IepEnrolledPipe,
    OrderByPipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    AdministrationRoutingModule,
    NgbModule
  ],
  providers: [ ]
})

export class StudentsModule { }
