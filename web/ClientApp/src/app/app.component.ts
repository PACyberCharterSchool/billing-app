import { Component } from '@angular/core';

import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'PACBill';

  public constructor(private router: Router) {
    console.log('AppComponent.ctor():  routes are ', this.router.config);
  }
}
