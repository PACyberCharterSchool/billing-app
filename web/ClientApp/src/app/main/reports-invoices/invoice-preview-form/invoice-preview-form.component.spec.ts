import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { InvoicePreviewFormComponent } from './invoice-preview-form.component';
import { InvoiceExcelPreviewComponent } from '../invoice-excel-preview/invoice-excel-preview.component';
import { ExcelComponent } from '../excel/excel.component';

import { HotTableModule, HotTableRegisterer } from '@handsontable/angular';

import { NgbModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { ReportsService } from '../../../services/reports.service';
import { FileSaverService } from '../../../services/file-saver.service';

import { Globals } from '../../../globals';

xdescribe('InvoicePreviewFormComponent', () => {
  let component: InvoicePreviewFormComponent;
  let fixture: ComponentFixture<InvoicePreviewFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        InvoicePreviewFormComponent,
        InvoiceExcelPreviewComponent,
        ExcelComponent
      ],
      imports: [
        HotTableModule,
        NgbModule.forRoot()
      ],
      providers: [
        HotTableRegisterer,
        NgbActiveModal,
        ReportsService,
        FileSaverService,
        Globals,
        HttpClient,
        HttpHandler
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoicePreviewFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
