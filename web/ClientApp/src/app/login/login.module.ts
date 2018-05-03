import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { LoginRoutingModule } from './login-routing.module';

import { LoginComponent } from './login/login.component';
import { LoginPanelComponent } from './login-panel/login-panel.component';
import { LoginPanelFormComponent } from './login-panel-form/login-panel-form.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    LoginRoutingModule
  ],
  declarations: [
    LoginComponent,
    LoginPanelComponent,
    LoginPanelFormComponent
  ],
  providers: []
})

export class LoginModule { }
