import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';

import { LoginComponent } from './components/login/login.component';
import { PaymentsRefundsComponent } from './components/payments-refunds/payments-refunds.component';

const routes: Routes = [
  // { path: '', component: HomeComponent, pathMatch: 'full' },
  // { path: 'counter', component: CounterComponent },
  // { path: 'fetch-data', component: FetchDataComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'payments-refunds', component: PaymentsRefundsComponent },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [ RouterModule ]
})
export class AppRoutingModule { }
