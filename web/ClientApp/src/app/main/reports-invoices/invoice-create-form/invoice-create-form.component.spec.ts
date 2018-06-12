import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceCreateFormComponent } from './invoice-create-form.component';

describe('InvoiceCreateFormComponent', () => {
  let component: InvoiceCreateFormComponent;
  let fixture: ComponentFixture<InvoiceCreateFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoiceCreateFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceCreateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
