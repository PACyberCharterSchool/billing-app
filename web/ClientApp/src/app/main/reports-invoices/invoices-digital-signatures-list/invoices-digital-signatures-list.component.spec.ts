import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FormsModule } from '@angular/forms';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { InvoicesDigitalSignaturesListComponent } from './invoices-digital-signatures-list.component';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';

import { DigitalSignaturesService } from '../../../services/digital-signatures.service';
import { UtilitiesService } from '../../../services/utilities.service';

import { Globals } from '../../../globals';

import { NgbModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';

describe('InvoicesDigitalSignaturesListComponent', () => {
  let component: InvoicesDigitalSignaturesListComponent;
  let fixture: ComponentFixture<InvoicesDigitalSignaturesListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        InvoicesDigitalSignaturesListComponent,
        NormalizeFieldNamePipe,
        NormalizeFieldValuePipe,
        OrderByPipe
      ],
      imports: [
        FormsModule,
        NgbModule.forRoot()
      ],
      providers: [
        DigitalSignaturesService,
        Globals,
        HttpClient,
        HttpHandler,
        UtilitiesService,
        NgbModal
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoicesDigitalSignaturesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
