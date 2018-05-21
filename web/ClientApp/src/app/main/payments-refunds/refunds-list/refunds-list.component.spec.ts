import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';

import { RefundsListComponent } from './refunds-list.component';

import { UtilitiesService } from '../../../services/utilities.service';
import { RefundsService } from '../../../services/refunds.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { Globals } from '../../../globals';

import { NgbModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';

describe('RefundsListComponent', () => {
  let component: RefundsListComponent;
  let fixture: ComponentFixture<RefundsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RefundsListComponent, NormalizeFieldNamePipe, OrderByPipe, NormalizeFieldValuePipe ],
      providers: [ UtilitiesService, RefundsService, SchoolDistrictService, HttpClient, HttpHandler, NgbModal, Globals ],
      imports: [ FormsModule, NgbModule.forRoot(), RouterTestingModule ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RefundsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
