import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FormsModule } from '@angular/forms';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { AdministrationAuditListComponent } from './administration-audit-list.component';

import { InterpretAuditTypePipe } from '../../../pipes/interpret-audit-type.pipe';
import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';

import { Globals } from '../../../globals';

import { UtilitiesService } from '../../../services/utilities.service';
import { AuditRecordsService } from '../../../services/audit-records.service'

xdescribe('AdministrationAuditListComponent', () => {
  let component: AdministrationAuditListComponent;
  let fixture: ComponentFixture<AdministrationAuditListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        AdministrationAuditListComponent,
        InterpretAuditTypePipe,
        NormalizeFieldNamePipe,
        NormalizeFieldValuePipe,
        OrderByPipe
      ],
      imports: [
        FormsModule
      ],
      providers: [
        Globals,
        HttpHandler,
        HttpClient,
        UtilitiesService,
        AuditRecordsService
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationAuditListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
