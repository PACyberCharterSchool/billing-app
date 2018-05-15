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
import { StudentStatusRecordsImportService } from './services/student-status-records-import.service';
import { PaymentsService } from './services/payments.service';
import { RefundsService } from './services/refunds.service';

import { TokenInterceptor } from './interceptors/token.interceptor';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

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
  AppTitleService,
  AuthenticationGuardService,
  AuthenticationService,
  CurrentStudentService,
  Globals,
  PaymentsService,
  RefundsService,
  SchoolDistrictService,
  StudentsService,
  StudentStatusRecordsImportService,
  tokenInterceptor,
  UtilitiesService,
];

@NgModule({
  declarations: pacbillDeclarations,
  imports: pacbillImports,
  providers: pacbillProviders,
  bootstrap: [ AppComponent ]
})
export class AppModule { }
