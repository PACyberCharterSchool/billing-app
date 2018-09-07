import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';

import { AppRoutingModule } from './app-routing.module';

import { LoginModule } from './login/login.module';
import { MainModule } from './main/main.module';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppTitleService } from './services/app-title.service';
import { AuthenticationService } from './services/authentication.service';
import { AuthenticationGuardService } from './services/authentication-guard.service';
import { StudentsService } from './services/students.service';
import { SchoolDistrictService } from './services/school-district.service';
import { UtilitiesService } from './services/utilities.service';
import { CurrentStudentService } from './services/current-student.service';
import { StudentRecordsService } from './services/student-records.service';
import { PaymentsService } from './services/payments.service';
import { RefundsService } from './services/refunds.service';
import { AcademicYearsService } from './services/academic-years.service';
import { SchoolCalendarService } from './services/school-calendar.service';
import { DigitalSignaturesService } from './services/digital-signatures.service';
import { AuditRecordsService } from './services/audit-records.service';
import { ReportsService } from './services/reports.service';
import { TemplatesService } from './services/templates.service';
import { SpreadsheetService } from './services/spreadsheet.service';
import { FileSaverService } from './services/file-saver.service';

import { TokenInterceptor } from './interceptors/token.interceptor';

import { NgbModule, NgbDatepickerConfig, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgbPACDateParserFormatter } from './shared/ngb-pac-date-parser-formatter';


import { Globals } from './globals';

const tokenInterceptor = {
  provide: HTTP_INTERCEPTORS,
  useClass: TokenInterceptor,
  multi: true
};

const pacbillDeclarations = [
  AppComponent
];

const pacbillImports = [
  BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
  BrowserAnimationsModule,
  HttpClientModule,
  FormsModule,
  RouterModule,
  LoginModule,
  MainModule,
  AppRoutingModule,
  NgbModule.forRoot()
];

const pacbillProviders = [
  AcademicYearsService,
  AppTitleService,
  AuditRecordsService,
  AuthenticationGuardService,
  AuthenticationService,
  CurrentStudentService,
  DigitalSignaturesService,
  FileSaverService,
  Globals,
  PaymentsService,
  RefundsService,
  SchoolCalendarService,
  SchoolDistrictService,
  StudentsService,
  StudentRecordsService,
  tokenInterceptor,
  UtilitiesService,
  ReportsService,
  TemplatesService,
  SpreadsheetService,
  { provide: NgbDateParserFormatter, useClass: NgbPACDateParserFormatter }
];

@NgModule({
  declarations: pacbillDeclarations,
  imports: pacbillImports,
  providers: pacbillProviders,
  bootstrap: [ AppComponent ]
})
export class AppModule { }
