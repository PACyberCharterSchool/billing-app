import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsInvoicesHomeComponent } from './reports-invoices-home.component';

describe('ReportsInvoicesHomeComponent', () => {
  let component: ReportsInvoicesHomeComponent;
  let fixture: ComponentFixture<ReportsInvoicesHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportsInvoicesHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportsInvoicesHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
