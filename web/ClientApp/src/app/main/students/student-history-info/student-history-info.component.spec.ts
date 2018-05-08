import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { FormsModule } from '@angular/forms';

import { StudentHistoryInfoComponent } from './student-history-info.component';
import { StudentActivityHistoryComponent } from '../student-activity-history/student-activity-history.component';

import { NgbModule, NgbTabsetConfig, NgbDropdownConfig } from '@ng-bootstrap/ng-bootstrap';

import { OrderByPipe } from '../../../pipes/orderby.pipe';
import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';

import { UtilitiesService } from '../../../services/utilities.service';
import { StudentsService } from '../../../services/students.service';
import { CurrentStudentService } from '../../../services/current-student.service';

import { Globals } from '../../../globals';

describe('StudentHistoryInfoComponent', () => {
  let component: StudentHistoryInfoComponent;
  let fixture: ComponentFixture<StudentHistoryInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        StudentHistoryInfoComponent,
        StudentActivityHistoryComponent,
        OrderByPipe,
        NormalizeFieldNamePipe
      ],
      imports: [ NgbModule, FormsModule ],
      providers: [
        NgbDropdownConfig,
        NgbTabsetConfig,
        HttpClient,
        HttpHandler,
        Globals,
        StudentsService,
        UtilitiesService,
        CurrentStudentService
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentHistoryInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
