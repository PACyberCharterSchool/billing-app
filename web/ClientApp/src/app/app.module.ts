import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';

import { LoginModule } from './login/login.module';
import { HomeModule } from './home/home.module';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppTitleService } from './services/app-title.service';
import { AuthenticationService } from './services/authentication.service';
import { AuthenticationGuardService } from './services/authentication-guard.service';

import { AppRoutingModule } from './app-routing.module';

const pacbillDeclarations = [
    AppComponent,
];

const pacbillImports = [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    LoginModule,
    HomeModule,
    AppRoutingModule,
    RouterModule
];

const pacbillProviders = [
  AppTitleService,
  AuthenticationGuardService,
  AuthenticationService
];

@NgModule({
  declarations: pacbillDeclarations,
  imports: pacbillImports,
  providers: pacbillProviders,
  bootstrap: [ AppComponent ]
})
export class AppModule { }
