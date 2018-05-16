import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { FormsModule } from '@angular/forms';

import { RefundUpsertFormComponent } from './refund-upsert-form.component';

import {
  NgbModule,
  NgbTypeahead,
  NgbTypeaheadConfig,
  NgbActiveModal,
  NgbCalendar,
  NgbDateParserFormatter,
  NgbDateAdapter,
  NgbDropdownConfig
} from '@ng-bootstrap/ng-bootstrap';

import { RefundsService } from '../../../services/refunds.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

describe('RefundUpsertFormComponent', () => {
  let component: RefundUpsertFormComponent;
  let fixture: ComponentFixture<RefundUpsertFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RefundUpsertFormComponent ],
      providers: [
        NgbTypeahead,
        NgbTypeaheadConfig,
        NgbCalendar,
        NgbActiveModal,
        NgbDateParserFormatter,
        NgbDateAdapter,
        RefundsService,
        SchoolDistrictService,
        HttpClient,
        HttpHandler,
        NgbDropdownConfig
      ],
      imports: [ FormsModule, NgbModule ]
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
