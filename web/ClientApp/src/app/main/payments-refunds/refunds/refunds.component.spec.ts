import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';

import { RefundsComponent } from './refunds.component';
import { RefundsListComponent } from '../refunds-list/refunds-list.component';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';

import { UtilitiesService } from '../../../services/utilities.service';
import { RefundsService } from '../../../services/refunds.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { Globals } from '../../../globals';

describe('RefundsComponent', () => {
  let component: RefundsComponent;
  let fixture: ComponentFixture<RefundsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        RefundsComponent,
        RefundsListComponent,
        NormalizeFieldNamePipe,
        OrderByPipe,
        NormalizeFieldValuePipe,
      ],
      providers: [ UtilitiesService, RefundsService, SchoolDistrictService, HttpClient, HttpHandler, Globals
      ],
      imports: [ FormsModule, NgbModule.forRoot(), RouterTestingModule ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RefundsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
