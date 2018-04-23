import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ContentAreaModule } from './content-area/content-area.module';

import { MainHomeRoutingModule } from './main-home-routing.module';

import { MainHomeHomeComponent } from './main-home-home/main-home-home.component';

import { LoginPopupComponent } from '../title-bar/login-popup/login-popup.component';
import { LoginTitleBarComponent } from '../title-bar/login-title-bar/login-title-bar.component';
import { TitleBarComponent } from '../title-bar/title-bar/title-bar.component';
import { FooterComponent } from '../footer/footer.component';

// Angular Material components
import { MatMenuModule, MatIconModule, MatButtonModule, MatSidenavModule } from '@angular/material';
import { MatFormFieldModule, MatFormFieldControl, MatInputModule } from '@angular/material';
// import { MAT_MENU_DEFAULT_OPTIONS } from '@angular/material';
import { MatMenu } from '@angular/material';

@NgModule({
  declarations: [
    MainHomeHomeComponent,
    TitleBarComponent,
    FooterComponent,
    LoginPopupComponent,
    LoginTitleBarComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ContentAreaModule,
    MainHomeRoutingModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    MatSidenavModule
  ],
  providers: []
})

export class MainHomeModule { }
