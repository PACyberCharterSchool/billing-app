import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoicePreviewFormComponent } from './invoice-preview-form.component';

describe('InvoicePreviewFormComponent', () => {
  let component: InvoicePreviewFormComponent;
  let fixture: ComponentFixture<InvoicePreviewFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoicePreviewFormComponent ]
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
