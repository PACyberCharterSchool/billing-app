import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentsRefundsComponent } from './payments-refunds.component';

describe('PaymentsRefundsComponent', () => {
  let component: PaymentsRefundsComponent;
  let fixture: ComponentFixture<PaymentsRefundsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentsRefundsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentsRefundsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
