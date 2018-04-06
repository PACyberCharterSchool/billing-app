import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { LoginComponent } from './login/login.component';
import { TitleBarComponent } from './title-bar/title-bar.component';
import { ContentAreaComponent } from './content-area/content-area.component';
import { FooterComponent } from './footer/footer.component';
import { ActionContentComponent } from './action-content/action-content.component';
import { LoginPopupComponent } from './login-popup/login-popup.component';

// Angular Material components
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatMenuModule, MatIconModule, MatButtonModule, MatSidenavModule } from '@angular/material';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginComponent,
    TitleBarComponent,
    ContentAreaComponent,
    FooterComponent,
    ActionContentComponent,
    LoginPopupComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      // { path: '', component: HomeComponent, pathMatch: 'full' },
      // { path: 'counter', component: CounterComponent },
      // { path: 'fetch-data', component: FetchDataComponent },
    ]),
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatSidenavModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
