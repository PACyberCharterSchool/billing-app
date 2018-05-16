import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClient, HttpHandler } from '@angular/common/http';

import { FormsModule } from '@angular/forms';

import { PaymentUpsertFormComponent } from './payment-upsert-form.component';

import { Globals } from '../../../globals';

import { PaymentsService } from '../../../services/payments.service';
import { SchoolDistrictService } from '../../../services/school-district.service';
import { AcademicYearsService } from '../../../services/academic-years.service';

import {
  NgbModule,
  NgbCalendar,
  NgbDateAdapter,
  NgbDateParserFormatter,
  NgbTypeahead,
  NgbTypeaheadConfig,
  NgbDropdownConfig,
  NgbActiveModal
} from '@ng-bootstrap/ng-bootstrap';

describe('PaymentUpsertFormComponent', () => {
  let component: PaymentUpsertFormComponent;
  let fixture: ComponentFixture<PaymentUpsertFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentUpsertFormComponent ],
      imports: [ NgbModule, FormsModule ],
      providers: [
        NgbTypeahead,
        NgbTypeaheadConfig,
        NgbDateAdapter,
        NgbDropdownConfig,
        NgbCalendar,
        NgbDateParserFormatter,
        NgbActiveModal,
        AcademicYearsService,
        PaymentsService,
        SchoolDistrictService,
        Globals,
        HttpClient,
        HttpHandler
      ]
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
