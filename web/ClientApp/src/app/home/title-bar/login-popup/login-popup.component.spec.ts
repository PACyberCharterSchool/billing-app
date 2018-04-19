import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { MatMenuModule, MatIconModule } from '@angular/material';

import { LoginPopupComponent } from '../login-popup/login-popup.component';
import { AuthenticationService } from '../../../../services/authentication.service';

describe(LoginPopupComponent.name, () => {
  let component: LoginPopupComponent;
  let fixture: ComponentFixture<LoginPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        LoginPopupComponent
      ],
      imports: [
        MatIconModule,
        MatMenuModule,
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
    fixture = TestBed.createComponent(LoginPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
