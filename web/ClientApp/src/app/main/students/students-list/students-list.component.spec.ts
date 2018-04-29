import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RouterTestingModule } from '@angular/router/testing';

import { FormsModule } from '@angular/forms';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { StudentsListComponent } from './students-list.component';
import { StudentDatepickerComponent } from '../student-datepicker/student-datepicker.component';
import { StudentAdvancedFilterComponent } from '../student-advanced-filter/student-advanced-filter.component';

import { StudentsService } from '../../../services/students.service';
import { SchoolDistrictService } from '../../../services/school-district.service';


import { OrderByPipe } from '../../../pipes/orderby.pipe';
import { IepEnrolledPipe } from '../../../pipes/iep-enrolled.pipe';

import { NgbModule, NgbDateAdapter, NgbDatepicker, NgbDateParserFormatter, NgbCalendar } from '@ng-bootstrap/ng-bootstrap';

describe('StudentsListComponent', () => {
  let component: StudentsListComponent;
  let fixture: ComponentFixture<StudentsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        StudentsListComponent,
        StudentDatepickerComponent,
        StudentAdvancedFilterComponent,
        OrderByPipe,
        IepEnrolledPipe
      ],
      imports: [ FormsModule, NgbModule, RouterTestingModule ],
      providers: [
        NgbCalendar,
        NgbDateAdapter,
        NgbDateParserFormatter,
        NgbDatepicker,
        SchoolDistrictService,
        StudentsService,
        HttpClient,
        HttpHandler
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
