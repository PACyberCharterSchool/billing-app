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
import { SortService } from './services/sort.service';

import { TokenInterceptor } from './interceptors/token.interceptor';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

const tokenInterceptor = {
  provide: HTTP_INTERCEPTORS,
  useClass: TokenInterceptor,
  multi: true
};

const pacbillDeclarations = [
  AppComponent
];

const pacbillImports = [
  NgbModule.forRoot(),
  BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
  BrowserAnimationsModule,
  HttpClientModule,
  FormsModule,
  RouterModule,
  LoginModule,
  MainModule,
  AppRoutingModule
];

const pacbillProviders = [
  AppTitleService,
  AuthenticationGuardService,
  AuthenticationService,
  StudentsService,
  tokenInterceptor
];

@NgModule({
  declarations: pacbillDeclarations,
  imports: pacbillImports,
  providers: pacbillProviders,
  bootstrap: [ AppComponent ]
})
export class AppModule { }
