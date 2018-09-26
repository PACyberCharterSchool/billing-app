import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountsReceivableAgingComponent } from './accounts-receivable-aging.component';

describe('AccountsReceivableAgingComponent', () => {
  let component: AccountsReceivableAgingComponent;
  let fixture: ComponentFixture<AccountsReceivableAgingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountsReceivableAgingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountsReceivableAgingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
