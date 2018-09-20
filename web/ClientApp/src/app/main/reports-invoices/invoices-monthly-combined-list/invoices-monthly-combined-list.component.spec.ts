import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { FormsModule } from '@angular/forms';

import { RouterTestingModule } from '@angular/router/testing';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';

import { Globals } from '../../../globals';

import { InvoicesMonthlyCombinedListComponent } from './invoices-monthly-combined-list.component';

import { ReportsService } from '../../../services/reports.service';
import { UtilitiesService } from '../../../services/utilities.service';
import { FileSaverService } from '../../../services/file-saver.service';
import { TemplatesService } from '../../../services/templates.service';
import { AcademicYearsService } from '../../../services/academic-years.service';
import { StudentRecordsService } from '../../../services/student-records.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { NgbModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { NgxSpinnerModule } from 'ngx-spinner';

describe('InvoicesMonthlyCombinedListComponent', () => {
  let component: InvoicesMonthlyCombinedListComponent;
  let fixture: ComponentFixture<InvoicesMonthlyCombinedListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        InvoicesMonthlyCombinedListComponent,
        NormalizeFieldNamePipe,
        NormalizeFieldValuePipe,
        OrderByPipe
      ],
      imports: [
        FormsModule,
        RouterTestingModule,
        NgbModule.forRoot(),
        NgxSpinnerModule
      ],
      providers: [
        Globals,
        ReportsService,
        UtilitiesService,
        FileSaverService,
        TemplatesService,
        AcademicYearsService,
        StudentRecordsService,
        SchoolDistrictService,
        HttpClient,
        HttpHandler,
        NgbActiveModal
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoicesMonthlyCombinedListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
