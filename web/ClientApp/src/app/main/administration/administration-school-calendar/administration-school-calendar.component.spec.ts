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

import { Globals } from '../../../globals';

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
      imports: [ FormsModule ],
      providers: [ SchoolCalendarService, UtilitiesService, HttpClient, HttpHandler, Globals ]
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
