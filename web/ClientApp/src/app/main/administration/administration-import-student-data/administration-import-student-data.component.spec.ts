import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { RouterTestingModule } from '@angular/router/testing';

import { AdministrationImportStudentDataComponent } from './administration-import-student-data.component';
import { NormalizeFieldNamePipe } from '../../../pipes/normalize-field-name.pipe';

import { StudentsService } from '../../../services/students.service';
import { Token } from '@angular/compiler';

describe(AdministrationImportStudentDataComponent.name, () => {
  // let component: AdministrationImportStudentDataComponent;
  // let fixture: ComponentFixture<AdministrationImportStudentDataComponent>;
  // let service: StudentsService;

  // beforeEach(async(() => {
  //   TestBed.configureTestingModule({
  //     declarations: [ AdministrationImportStudentDataComponent, NormalizeFieldNamePipe  ],
  //     imports: [ RouterTestingModule ],
  //     providers: [ StudentsService, HttpClient, HttpHandler ]
  //   })
  //   .compileComponents();
  // }));

  // beforeEach(() => {
  //   fixture = TestBed.createComponent(AdministrationImportStudentDataComponent);
  //   component = fixture.componentInstance;
  //   fixture.detectChanges();
  //   service = TestBed.get(StudentsService);
  // });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
