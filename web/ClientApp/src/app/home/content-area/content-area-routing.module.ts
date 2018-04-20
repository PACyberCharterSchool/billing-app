import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ContentAreaComponent } from './content-area.component';

const contentAreaRoutes: Routes = [
  {
    path: '',
    component: ContentAreaComponent,
    children: [
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
