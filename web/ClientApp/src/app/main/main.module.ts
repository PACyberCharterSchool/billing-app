import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MainRoutingModule } from './main-routing.module';

import { StudentsModule } from './students/students.module';
import { AdministrationModule } from './administration/administration.module';

import { MainComponent } from './main.component';
import { ContentAreaComponent } from './content-area/content-area.component';
import { FooterComponent } from './footer/footer.component';
import { LoginPopupComponent } from './title-bar/login-popup/login-popup.component';
import { LoginTitleBarComponent } from './title-bar/login-title-bar/login-title-bar.component';
import { TitleBarComponent } from './title-bar/title-bar/title-bar.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

// Angular Material components
import { MatMenuModule, MatIconModule, MatButtonModule, MatSidenavModule } from '@angular/material';
import { MatFormFieldModule, MatFormFieldControl, MatInputModule } from '@angular/material';
import { MatMenu } from '@angular/material';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    ContentAreaComponent,
    FooterComponent,
    LoginPopupComponent,
    LoginTitleBarComponent,
    MainComponent,
    TitleBarComponent,
    NavMenuComponent
  ],
  exports: [],
  imports: [
    CommonModule,
    FormsModule,
    MainRoutingModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    MatSidenavModule,
    NgbModule,
    StudentsModule,
    AdministrationModule
  ],
  providers: []
})

export class MainModule { }
