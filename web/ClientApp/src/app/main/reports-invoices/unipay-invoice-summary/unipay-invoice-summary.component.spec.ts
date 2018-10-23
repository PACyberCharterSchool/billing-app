import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UnipayInvoiceSummaryComponent } from './unipay-invoice-summary.component';

describe('UnipayInvoiceSummaryComponent', () => {
  let component: UnipayInvoiceSummaryComponent;
  let fixture: ComponentFixture<UnipayInvoiceSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UnipayInvoiceSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UnipayInvoiceSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
