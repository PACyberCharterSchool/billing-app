import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
// import { BrowserModule } from '@angular/platform-browser';

import { ContentAreaModule } from './content-area/content-area.module';

import { HomeRoutingModule } from './home-routing.module';

import { HomeComponent } from './home/home.component';
import { NavMenuComponent } from './content-area/nav-menu/nav-menu.component';
import { ActionContentComponent } from './content-area/action-content/action-content.component';
import { ContentAreaComponent } from './content-area/content-area.component';
import { LoginPopupComponent } from './title-bar/login-popup/login-popup.component';
import { LoginTitleBarComponent } from './title-bar/login-title-bar/login-title-bar.component';
import { TitleBarComponent } from './title-bar/title-bar/title-bar.component';
import { FooterComponent } from './footer/footer/footer.component';

// Angular Material components
import { MatMenuModule, MatIconModule, MatButtonModule, MatSidenavModule } from '@angular/material';
import { MatFormFieldModule, MatFormFieldControl, MatInputModule } from '@angular/material';
import { MAT_MENU_DEFAULT_OPTIONS } from '@angular/material';
import { MatMenu } from '@angular/material';

@NgModule({
  declarations: [
    ContentAreaComponent,
    FooterComponent,
    HomeComponent,
    LoginPopupComponent,
    LoginTitleBarComponent,
    NavMenuComponent,
    ActionContentComponent,
    TitleBarComponent
  ],
  imports: [
    // BrowserModule,
    CommonModule,
    FormsModule,
    ContentAreaModule,
    HomeRoutingModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    MatSidenavModule
  ],
  providers: []
})

export class HomeModule { }
