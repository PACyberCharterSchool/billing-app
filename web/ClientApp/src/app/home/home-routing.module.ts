import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './home/home.component';

import { AuthenticationGuardService } from '../services/authentication-guard.service';

const homeRoutes: Routes = [
  {
    path: '',
    component: HomeComponent,
    canActivate: [ AuthenticationGuardService ]
  }
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(homeRoutes)
  ],
  exports: [
    RouterModule
  ]
})

export class HomeRoutingModule { }
