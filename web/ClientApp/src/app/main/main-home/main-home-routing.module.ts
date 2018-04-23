import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainHomeComponent } from './main-home.component';

import { MainHomeHomeComponent } from './main-home-home/main-home-home.component';

const mainHomeRoutes: Routes = [
  {
    path: 'main-home',
    component: MainHomeComponent,
    children: [
      {
        path: '',
        component: MainHomeHomeComponent
      }
    ]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(mainHomeRoutes)
  ],
  exports: [
    RouterModule
  ],
  declarations: [],
  providers: []
})

export class MainHomeRoutingModule { }
