import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MainRoutingModule } from './main-routing.module';

import { StudentsModule } from './students/students.module';
import { AdministrationModule } from './administration/administration.module';
import { ReportsInvoicesModule } from './reports-invoices/reports-invoices.module';
import { PaymentsRefundsModule } from './payments-refunds/payments-refunds.module';

import { MainComponent } from './main.component';
import { ContentAreaComponent } from './content-area/content-area.component';
import { FooterComponent } from './footer/footer.component';
import { LoginPopupComponent } from './title-bar/login-popup/login-popup.component';
import { LoginTitleBarComponent } from './title-bar/login-title-bar/login-title-bar.component';
import { TitleBarComponent } from './title-bar/title-bar/title-bar.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

// Angular Material components
import { MatMenuModule, MatIconModule, MatButtonModule, MatSidenavModule } from '@angular/material';
import { MatFormFieldModule, MatFormFieldControl, MatInputModule } from '@angular/material';
import { MatMenu } from '@angular/material';
import { NgxCurrencyModule } from 'ngx-currency';
import { CurrencyMaskConfig, CURRENCY_MASK_CONFIG } from 'ngx-currency/src/currency-mask.config';

export const CustomCurrencyMaskConfig: CurrencyMaskConfig = {
    align: 'left',
    allowNegative: false,
    allowZero: true,
    decimal: '.',
    precision: 2,
    prefix: '$',
    suffix: '',
    thousands: ',',
    nullable: true
};

@NgModule({
  declarations: [
    ContentAreaComponent,
    FooterComponent,
    LoginPopupComponent,
    LoginTitleBarComponent,
    MainComponent,
    TitleBarComponent,
    NavMenuComponent
  ],
  exports: [],
  imports: [
    CommonModule,
    FormsModule,
    MainRoutingModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    MatSidenavModule,
    StudentsModule,
    AdministrationModule,
    ReportsInvoicesModule,
    PaymentsRefundsModule,
    NgxCurrencyModule
  ],
  providers: [
    { provide: CURRENCY_MASK_CONFIG, useValue: CustomCurrencyMaskConfig }
  ]
})

export class MainModule { }
