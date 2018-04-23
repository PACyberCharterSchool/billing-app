import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ContentAreaComponent } from './content-area.component';
import { ContentAreaHomeComponent } from './content-area-home/content-area-home.component';

const contentAreaRoutes: Routes = [
  {
    path: '',
    component: ContentAreaComponent,
    children: [
      {
        path: '',
        component: ContentAreaHomeComponent
      }
    ]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(contentAreaRoutes)
  ],
  exports: [
    RouterModule
  ],
  declarations: [],
  providers: []
})

export class ContentAreaRoutingModule { }
