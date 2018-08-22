import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivatedRoute, Router } from '@angular/router';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { RouterTestingModule } from '@angular/router/testing';

import { StudentDetailsInfoComponent } from './student-details-info.component';

import { CurrentStudentService } from '../../../services/current-student.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { StudentRecordsService } from '../../../services/student-records.service';

import { Globals } from '../../../globals';

import {
  NgbModule,
  NgbDateAdapter,
  NgbDatepicker,
  NgbDateParserFormatter,
  NgbCalendar,
  NgbDropdownConfig,
  NgbModal } from '@ng-bootstrap/ng-bootstrap';


xdescribe('StudentDetailsInfoComponent', () => {
  let component: StudentDetailsInfoComponent;
  let fixture: ComponentFixture<StudentDetailsInfoComponent>;
  const fakeActivatedRoute = {
    snapshot: { data: {} }
  } as ActivatedRoute;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentDetailsInfoComponent ],
      providers: [
        NgbCalendar,
        NgbDateAdapter,
        NgbDateParserFormatter,
        NgbDatepicker,
        NgbDropdownConfig,
        CurrentStudentService,
        SchoolDistrictService,
        HttpClient,
        HttpHandler,
        StudentRecordsService,
        Globals,
        {
          provide: ActivatedRoute,
          useValue: fakeActivatedRoute
        },
        {
          provide: Router,
          useClass: class { navigate = jasmine.createSpy('navigate'); }
        },
        NgbModal
      ],
      imports: [ FormsModule, ReactiveFormsModule, NgbModule.forRoot() ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentDetailsInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
