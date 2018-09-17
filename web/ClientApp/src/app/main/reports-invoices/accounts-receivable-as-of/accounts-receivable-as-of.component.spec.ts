import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountsReceivableAsOfComponent } from './accounts-receivable-as-of.component';

describe('AccountsReceivableAsOfComponent', () => {
  let component: AccountsReceivableAsOfComponent;
  let fixture: ComponentFixture<AccountsReceivableAsOfComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountsReceivableAsOfComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountsReceivableAsOfComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
