import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHandler } from '@angular/common/http';

import { PaymentsComponent } from './payments.component';
import { PaymentsListComponent } from '../payments-list/payments-list.component';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';

import { Globals } from '../../../globals';

import { UtilitiesService } from '../../../services/utilities.service';
import { PaymentsService } from '../../../services/payments.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { InfiniteScrollModule } from 'ngx-infinite-scroll';

import { NgbModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';

describe('PaymentsComponent', () => {
  let component: PaymentsComponent;
  let fixture: ComponentFixture<PaymentsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentsComponent, PaymentsListComponent, NormalizeFieldNamePipe, NormalizeFieldValuePipe, OrderByPipe ],
      imports: [ FormsModule, InfiniteScrollModule, NgbModule.forRoot() ],
      providers: [ Globals, UtilitiesService, PaymentsService, SchoolDistrictService, HttpClient, HttpHandler, NgbModal ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
