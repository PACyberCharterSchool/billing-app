import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministrationPaymentRateUpdateComponent } from './administration-payment-rate-update.component';

describe('AdministrationPaymentRateUpdateComponent', () => {
  let component: AdministrationPaymentRateUpdateComponent;
  let fixture: ComponentFixture<AdministrationPaymentRateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdministrationPaymentRateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationPaymentRateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
