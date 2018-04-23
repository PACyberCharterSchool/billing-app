import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MainHomeModule } from './main-home/main-home.module';

import { MainRoutingModule } from './main-routing.module';

import { MainHomeComponent } from './main-home/main-home.component';
import { MainComponent } from './main.component';
import { ContentAreaComponent } from './main-home/content-area/content-area.component';

@NgModule({
  declarations: [
    ContentAreaComponent,
    // FooterComponent,
    MainHomeComponent,
    // LoginPopupComponent,
    // LoginTitleBarComponent,
    MainComponent,
    MainHomeComponent,
    // TitleBarComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    MainHomeModule,
    MainRoutingModule
  ],
  providers: []
})

export class MainModule { }
