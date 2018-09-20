import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FormsModule } from '@angular/forms';

import { HttpClient, HttpHandler } from '@angular/common/http';

import {
  AdministrationSchoolCalendarComponent
} from './administration-school-calendar.component';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';

import { SchoolCalendarService } from '../../../services/school-calendar.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { AcademicYearsService } from '../../../services/academic-years.service';

import { Globals } from '../../../globals';

import { NgxSpinnerModule } from 'ngx-spinner';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

describe('AdministrationSchoolCalendarComponent', () => {
  let component: AdministrationSchoolCalendarComponent;
  let fixture: ComponentFixture<AdministrationSchoolCalendarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        AdministrationSchoolCalendarComponent,
        NormalizeFieldNamePipe,
        NormalizeFieldValuePipe,
        OrderByPipe
      ],
      imports: [ FormsModule, NgxSpinnerModule, NgbModule.forRoot() ],
      providers: [ SchoolCalendarService, UtilitiesService, HttpClient, HttpHandler, Globals, AcademicYearsService ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationSchoolCalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
