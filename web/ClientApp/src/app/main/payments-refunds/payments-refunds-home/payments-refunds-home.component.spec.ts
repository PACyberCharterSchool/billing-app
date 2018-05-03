import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentsRefundsHomeComponent } from './payments-refunds-home.component';

describe('PaymentsRefundsHomeComponent', () => {
  let component: PaymentsRefundsHomeComponent;
  let fixture: ComponentFixture<PaymentsRefundsHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentsRefundsHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentsRefundsHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
