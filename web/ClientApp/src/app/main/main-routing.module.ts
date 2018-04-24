import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainComponent } from './main.component';

import { AuthenticationGuardService } from '../services/authentication-guard.service';

const mainRoutes: Routes = [
  {
    path: 'main',
    component: MainComponent,
    canActivate: [ AuthenticationGuardService ]
  }
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(mainRoutes)
  ],
  exports: [
    RouterModule
  ]
})

export class MainRoutingModule { }
