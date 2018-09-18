import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';

import { AccountsReceivableAsOfComponent } from './accounts-receivable-as-of.component';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';

import {
  NgbModule,
  NgbDateAdapter,
  NgbDatepicker,
  NgbDateParserFormatter,
  NgbCalendar,
  NgbDropdownConfig } from '@ng-bootstrap/ng-bootstrap';

import { NgxSpinnerModule } from 'ngx-spinner';

xdescribe('AccountsReceivableAsOfComponent', () => {
  let component: AccountsReceivableAsOfComponent;
  let fixture: ComponentFixture<AccountsReceivableAsOfComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        AccountsReceivableAsOfComponent,
        NormalizeFieldNamePipe,
        NormalizeFieldValuePipe,
        OrderByPipe
      ],
      imports: [ FormsModule, NgxSpinnerModule ],
      providers: [
        NgbCalendar,
        NgbDateAdapter,
        NgbDateParserFormatter,
        NgbDatepicker,
        NgbDropdownConfig
      ]
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
