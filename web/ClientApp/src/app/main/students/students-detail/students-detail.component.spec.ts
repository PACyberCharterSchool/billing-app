import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RouterTestingModule } from '@angular/router/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { StudentsService } from '../../../services/students.service';

import { StudentsDetailComponent } from './students-detail.component';
import { StudentDetailsInfoComponent } from '../student-details-info/student-details-info.component';
import { StudentHistoryInfoComponent } from '../student-history-info/student-history-info.component';
import { StudentAddressHistoryComponent } from '../student-address-history/student-address-history.component';
import { StudentEnrollmentHistoryComponent } from '../student-enrollment-history/student-enrollment-history.component';

import { NgbModule, NgbTabset, NgbTabsetConfig } from '@ng-bootstrap/ng-bootstrap';

describe('StudentsDetailComponent', () => {
  let component: StudentsDetailComponent;
  let fixture: ComponentFixture<StudentsDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        StudentsDetailComponent,
        StudentDetailsInfoComponent,
        StudentHistoryInfoComponent,
        StudentAddressHistoryComponent,
        StudentEnrollmentHistoryComponent
      ],
      imports: [ NgbModule, RouterTestingModule ],
      providers: [
        StudentsService,
        HttpClient,
        HttpHandler,
        NgbTabset,
        NgbTabsetConfig
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
