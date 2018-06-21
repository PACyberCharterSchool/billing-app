import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceExcelPreviewComponent } from './invoice-excel-preview.component';

import { ExcelComponent } from '../excel/excel.component';

import { HotTableModule, HotTableRegisterer } from '@handsontable/angular';

xdescribe('InvoiceExcelPreviewComponent', () => {
  let component: InvoiceExcelPreviewComponent;
  let fixture: ComponentFixture<InvoiceExcelPreviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        InvoiceExcelPreviewComponent,
        ExcelComponent
      ],
      imports: [
        HotTableModule
      ],
      providers: [
        HotTableRegisterer
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceExcelPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
