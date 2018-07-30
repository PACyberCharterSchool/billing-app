import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';

import { AdministrationTemplateListComponent } from './administration-template-list.component';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { NormalizeFieldValuePipe } from '../../../pipes/normalize-field-value.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';

import { Globals } from '../../../globals';
import { UtilitiesService } from '../../../services/utilities.service';
import { TemplatesService } from '../../../services/templates.service';
import { AcademicYearsService } from '../../../services/academic-years.service';

import { NgbModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';

describe('AdministrationTemplateListComponent', () => {
  let component: AdministrationTemplateListComponent;
  let fixture: ComponentFixture<AdministrationTemplateListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        AdministrationTemplateListComponent,
        NormalizeFieldNamePipe,
        NormalizeFieldValuePipe,
        OrderByPipe
      ],
      imports: [
        FormsModule,
        RouterTestingModule,
        NgbModule.forRoot()
      ],
      providers: [
        Globals,
        UtilitiesService,
        TemplatesService,
        AcademicYearsService,
        HttpClient,
        HttpHandler,
        NgbModal
      ],

    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationTemplateListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
