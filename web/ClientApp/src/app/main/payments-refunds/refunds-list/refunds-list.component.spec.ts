import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';
import { OrderByPipe } from '../../../pipes/orderby.pipe';

import { RefundsListComponent } from './refunds-list.component';

import { UtilitiesService } from '../../../services/utilities.service';
import { RefundsService } from '../../../services/refunds.service';
import { SchoolDistrictService } from '../../../services/school-district.service';

import { NgbModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';

describe('RefundsListComponent', () => {
  let component: RefundsListComponent;
  let fixture: ComponentFixture<RefundsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RefundsListComponent, NormalizeFieldNamePipe, OrderByPipe ],
      providers: [ UtilitiesService, RefundsService, SchoolDistrictService, HttpClient, HttpHandler, NgbModal ],
      imports: [ FormsModule, NgbModule.forRoot() ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RefundsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
