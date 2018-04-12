import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';
import { CounterComponent } from './components/counter/counter.component';
import { FetchDataComponent } from './components/fetch-data/fetch-data.component';
import { LoginTitleBarComponent } from './components/login-title-bar/login-title-bar.component';
import { TitleBarComponent } from './components/title-bar/title-bar.component';
import { ContentAreaComponent } from './components/content-area/content-area.component';
import { FooterComponent } from './components/footer/footer.component';
import { ActionContentComponent } from './components/action-content/action-content.component';
import { LoginPopupComponent } from './components/login-popup/login-popup.component';
import { LoginComponent } from './components/login/login.component';
import { LoginPanelComponent } from './components/login-panel/login-panel.component';
import { LoginPanelFormComponent } from './components/login-panel-form/login-panel-form.component';
import { PaymentsRefundsComponent } from './components/payments-refunds/payments-refunds.component';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Angular Material components
import { MatMenuModule, MatIconModule, MatButtonModule, MatSidenavModule } from '@angular/material';
import { MatFormFieldModule, MatFormFieldControl, MatInputModule } from '@angular/material';
import { MAT_MENU_DEFAULT_OPTIONS } from '@angular/material';
import { MatMenu } from '@angular/material';

import { AppTitleService } from './services/app-title.service';
import { AuthenticationService } from './services/authentication.service';
import { AuthenticationGuardService } from './services/authentication-guard.service';

import { AppRoutingModule } from './app-routing.module';

const pacbillDeclarations = [
    AppComponent,
    NavMenuComponent,
    LoginComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginTitleBarComponent,
    TitleBarComponent,
    ContentAreaComponent,
    FooterComponent,
    ActionContentComponent,
    LoginPopupComponent,
    PaymentsRefundsComponent,
    LoginPanelComponent,
    LoginPanelFormComponent
];

const pacbillImports = [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    MatSidenavModule,
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
