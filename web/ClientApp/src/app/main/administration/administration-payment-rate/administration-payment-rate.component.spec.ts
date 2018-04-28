import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministrationPaymentRateComponent } from './administration-payment-rate.component';

describe('AdministrationPaymentRateComponent', () => {
  let component: AdministrationPaymentRateComponent;
  let fixture: ComponentFixture<AdministrationPaymentRateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdministrationPaymentRateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationPaymentRateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
