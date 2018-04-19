import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ContentAreaComponent } from './content-area.component';
import { ActionContentComponent } from './action-content/action-content.component';

const contentAreaRoutes: Routes = [
  {
    path: '',
    component: ContentAreaComponent,
    children: [
      {
        path: '',
        component: ActionContentComponent
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
