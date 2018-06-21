import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { RouterTestingModule } from '@angular/router/testing';

import { AdministrationImportStudentDataComponent } from './administration-import-student-data.component';
import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';

import { StudentsService } from '../../../services/students.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { StudentStatusRecordsImportService } from '../../../services/student-status-records-import.service';

import { Token } from '@angular/compiler';
import { NgxSpinnerModule } from 'ngx-spinner';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';

import { Globals } from '../../../globals';

describe(AdministrationImportStudentDataComponent.name, () => {
  let component: AdministrationImportStudentDataComponent;
  let fixture: ComponentFixture<AdministrationImportStudentDataComponent>;
  let service: StudentsService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        AdministrationImportStudentDataComponent,
        NormalizeFieldNamePipe,
        NormalizeFieldValuePipe,
        OrderByPipe
      ],
      imports: [ RouterTestingModule, NgxSpinnerModule, InfiniteScrollModule ],
      providers: [
        StudentsService,
        UtilitiesService,
        StudentStatusRecordsImportService,
        HttpClient,
        HttpHandler,
        Globals
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationImportStudentDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    service = TestBed.get(StudentsService);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
