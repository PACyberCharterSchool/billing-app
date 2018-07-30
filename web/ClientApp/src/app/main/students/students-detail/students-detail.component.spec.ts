import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RouterTestingModule } from '@angular/router/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { StudentsService } from '../../../services/students.service';
import { CurrentStudentService } from '../../../services/current-student.service';
import { UtilitiesService } from '../../../services/utilities.service';

import { StudentsDetailComponent } from './students-detail.component';
import { StudentDetailsInfoComponent } from '../student-details-info/student-details-info.component';
import { StudentHistoryInfoComponent } from '../student-history-info/student-history-info.component';
import { StudentActivityHistoryComponent } from '../student-activity-history/student-activity-history.component';

import { NgbModule, NgbTabset, NgbTabsetConfig, NgbDropdownConfig } from '@ng-bootstrap/ng-bootstrap';

import { OrderByPipe } from '../../../pipes/orderby.pipe';
import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';

import { Globals } from '../../../globals';

describe('StudentsDetailComponent', () => {
  let component: StudentsDetailComponent;
  let fixture: ComponentFixture<StudentsDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        StudentsDetailComponent,
        StudentDetailsInfoComponent,
        StudentHistoryInfoComponent,
        StudentActivityHistoryComponent,
        OrderByPipe,
        NormalizeFieldNamePipe,
        NormalizeFieldValuePipe
      ],
      imports: [ NgbModule, RouterTestingModule ],
      providers: [
        CurrentStudentService,
        StudentsService,
        UtilitiesService,
        HttpClient,
        HttpHandler,
        NgbTabset,
        NgbTabsetConfig,
        NgbDropdownConfig,
        Globals
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentsDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
