import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { RouterTestingModule } from '@angular/router/testing';

import { TitleBarComponent } from './title-bar.component';
import { LoginTitleBarComponent } from '../login-title-bar/login-title-bar.component';
import { LoginPopupComponent } from '../login-popup/login-popup.component';
import { AuthenticationService } from '../../../../services/authentication.service';

import { MatMenuModule, MatIconModule } from '@angular/material';

describe('TitleBarComponent', () => {
  let component: TitleBarComponent;
  let fixture: ComponentFixture<TitleBarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        LoginTitleBarComponent,
        TitleBarComponent,
        LoginPopupComponent
      ],
      imports: [
        MatMenuModule,
        MatIconModule,
        RouterTestingModule
      ],
      providers: [
        AuthenticationService,
        HttpClient,
        HttpHandler
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TitleBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
