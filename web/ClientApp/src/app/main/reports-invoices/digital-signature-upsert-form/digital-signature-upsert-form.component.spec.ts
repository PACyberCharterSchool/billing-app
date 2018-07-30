import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FormsModule } from '@angular/forms';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { NgbModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { DigitalSignatureUpsertFormComponent } from './digital-signature-upsert-form.component';

import { DigitalSignaturesService } from '../../../services/digital-signatures.service';

import { Globals } from '../../../globals';

describe('DigitalSignatureUpsertFormComponent', () => {
  let component: DigitalSignatureUpsertFormComponent;
  let fixture: ComponentFixture<DigitalSignatureUpsertFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DigitalSignatureUpsertFormComponent ],
      imports: [
        FormsModule,
        NgbModule.forRoot()
      ],
      providers: [
        NgbActiveModal,
        DigitalSignaturesService,
        Globals,
        HttpClient,
        HttpHandler
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DigitalSignatureUpsertFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
