import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { StudentActivityHistoryComponent } from './student-activity-history.component';

import { OrderByPipe } from '../../../pipes/orderby.pipe';
import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';

import { UtilitiesService } from '../../../services/utilities.service';
import { StudentsService } from '../../../services/students.service';
import { CurrentStudentService } from '../../../services/current-student.service';

import { Globals } from '../../../globals';

describe('StudentActivityHistoryComponent', () => {
  let component: StudentActivityHistoryComponent;
  let fixture: ComponentFixture<StudentActivityHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentActivityHistoryComponent, OrderByPipe, NormalizeFieldNamePipe, NormalizeFieldValuePipe ],
      providers: [ CurrentStudentService, UtilitiesService, StudentsService, HttpClient, HttpHandler, Globals ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentActivityHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
