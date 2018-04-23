import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ActionContentModule } from './action-content/action-content.module';

import { ContentAreaRoutingModule } from './content-area-routing.module';

import { ContentAreaHomeComponent } from './content-area-home/content-area-home.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ActionContentModule,
    ContentAreaRoutingModule
  ],
  declarations: [
    ContentAreaHomeComponent,
    NavMenuComponent
  ],
  providers: []
})

export class ContentAreaModule { }
