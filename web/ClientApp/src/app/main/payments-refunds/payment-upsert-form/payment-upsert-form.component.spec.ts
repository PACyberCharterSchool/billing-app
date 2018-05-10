import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentUpsertFormComponent } from './payment-upsert-form.component';

describe('PaymentUpsertFormComponent', () => {
  let component: PaymentUpsertFormComponent;
  let fixture: ComponentFixture<PaymentUpsertFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentUpsertFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentUpsertFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
