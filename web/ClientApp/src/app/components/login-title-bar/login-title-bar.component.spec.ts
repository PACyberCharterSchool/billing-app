import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RouterTestingModule } from '@angular/router/testing';

import { MatMenuModule, MatIconModule } from '@angular/material';
import { HttpClient, HttpHandler } from '@angular/common/http';

import { LoginTitleBarComponent } from './login-title-bar.component';
import { LoginPopupComponent } from '../login-popup/login-popup.component';
import { AuthenticationService } from '../../services/authentication.service';

describe(LoginTitleBarComponent.name, () => {
  let component: LoginTitleBarComponent;
  let fixture: ComponentFixture<LoginTitleBarComponent>;
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        LoginTitleBarComponent,
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
    fixture = TestBed.createComponent(LoginTitleBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
