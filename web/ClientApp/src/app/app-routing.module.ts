import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { PaymentsRefundsComponent } from './components/payments-refunds/payments-refunds.component';

import { AuthenticationGuardService } from './services/authentication-guard.service';

const routes: Routes = [
  // { path: '', component: HomeComponent, pathMatch: 'full' },
  // { path: 'counter', component: CounterComponent },
  // { path: 'fetch-data', component: FetchDataComponent },
  { path: '', component: HomeComponent, canActivate: [ AuthenticationGuardService ] },
  { path: 'login', component: LoginComponent },
  { path: 'payments-refunds', component: PaymentsRefundsComponent, canActivate: [ AuthenticationGuardService ] },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [ RouterModule ]
})
export class AppRoutingModule { }
