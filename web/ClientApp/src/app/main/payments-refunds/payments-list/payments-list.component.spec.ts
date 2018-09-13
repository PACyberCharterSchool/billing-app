import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { PaymentsListComponent } from './payments-list.component';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';

import { Globals } from '../../../globals';

import { InfiniteScrollModule } from 'ngx-infinite-scroll';

import { UtilitiesService } from '../../../services/utilities.service';
import { PaymentsService } from '../../../services/payments.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import {
  NgbModule,
  NgbModal
} from '@ng-bootstrap/ng-bootstrap';

import { NgxSpinnerModule } from 'ngx-spinner';

describe('PaymentsListComponent', () => {
  let component: PaymentsListComponent;
  let fixture: ComponentFixture<PaymentsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentsListComponent, NormalizeFieldNamePipe, NormalizeFieldValuePipe, OrderByPipe ],
      imports: [ FormsModule, InfiniteScrollModule, NgbModule.forRoot(), NgxSpinnerModule, RouterTestingModule ],
      providers: [
        Globals,
        UtilitiesService,
        PaymentsService,
        SchoolDistrictService,
        HttpClient,
        HttpHandler,
        NgbModal
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
