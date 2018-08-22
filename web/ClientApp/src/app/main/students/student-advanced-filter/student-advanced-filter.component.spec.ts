import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHandler } from '@angular/common/http';

import { StudentAdvancedFilterComponent } from './student-advanced-filter.component';
import { StudentDatepickerComponent } from '../student-datepicker/student-datepicker.component';

import { OrderByPipe } from '../../../pipes/orderby.pipe';
import { IepEnrolledPipe } from '../../../pipes/iep-enrolled.pipe';

import { StudentsService } from '../../../services/students.service';
import { StudentRecordsService } from '../../../services/student-records.service';

import { NgbModule, NgbCalendar, NgbDropdownConfig, NgbDateParserFormatter, NgbDateAdapter  } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerModule } from 'ngx-spinner';

import { Globals } from '../../../globals';

describe('StudentAdvancedFilterComponent', () => {
  let component: StudentAdvancedFilterComponent;
  let fixture: ComponentFixture<StudentAdvancedFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentAdvancedFilterComponent, StudentDatepickerComponent, IepEnrolledPipe, OrderByPipe ],
      imports: [ FormsModule, NgbModule, NgxSpinnerModule ],
      providers: [
        StudentsService,
        StudentRecordsService,
        HttpClient,
        HttpHandler,
        NgbDropdownConfig,
        NgbCalendar,
        NgbDateParserFormatter,
        NgbDateAdapter,
        Globals
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentAdvancedFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
