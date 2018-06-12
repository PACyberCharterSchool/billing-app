import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceExcelPreviewComponent } from './invoice-excel-preview.component';

describe('InvoiceExcelPreviewComponent', () => {
  let component: InvoiceExcelPreviewComponent;
  let fixture: ComponentFixture<InvoiceExcelPreviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoiceExcelPreviewComponent ]
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
