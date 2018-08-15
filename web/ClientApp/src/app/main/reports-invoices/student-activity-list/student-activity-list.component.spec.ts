import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';

import { StudentActivityListComponent } from './student-activity-list.component';

import { NgbModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';

import { OrderByPipe } from '../../../pipes/orderby.pipe';

import { UtilitiesService } from '../../../services/utilities.service';
import { ReportsService } from '../../../services/reports.service';

import { NgxSpinnerModule } from 'ngx-spinner';

import { Globals } from '../../../globals';

describe('StudentActivityListComponent', () => {
  let component: StudentActivityListComponent;
  let fixture: ComponentFixture<StudentActivityListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        StudentActivityListComponent,
        NormalizeFieldNamePipe,
        NormalizeFieldValuePipe,
        OrderByPipe
      ],
      imports: [
        NgbModule.forRoot(),
        FormsModule,
        RouterTestingModule,
        NgxSpinnerModule
      ],
      providers: [
        UtilitiesService,
        ReportsService,
        NgbModal,
        Globals,
        HttpClient,
        HttpHandler
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentActivityListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
