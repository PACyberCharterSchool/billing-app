import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RefundUpsertFormComponent } from './refund-upsert-form.component';

describe('RefundUpsertFormComponent', () => {
  let component: RefundUpsertFormComponent;
  let fixture: ComponentFixture<RefundUpsertFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RefundUpsertFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RefundUpsertFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
