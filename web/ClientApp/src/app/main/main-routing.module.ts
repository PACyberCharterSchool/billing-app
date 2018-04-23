import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainComponent } from './main.component';
import { MainHomeComponent } from './main-home/main-home.component';

import { AuthenticationGuardService } from '../services/authentication-guard.service';

const mainRoutes: Routes = [
  {
    path: 'main',
    component: MainComponent,
    canActivate: [ AuthenticationGuardService ],
    children: [
      {
        path: '',
        component: MainHomeComponent
      }
    ]
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
