import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { FormsModule } from '@angular/forms';

import { HttpClient, HttpHandler } from '@angular/common/http';

import { LoginPanelFormComponent } from './login-panel-form.component';
import { AuthenticationService } from '../../../services/authentication.service';

import { MatMenuModule, MatIconModule } from '@angular/material';

describe('LoginPanelFormComponent', () => {
  let component: LoginPanelFormComponent;
  let fixture: ComponentFixture<LoginPanelFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        LoginPanelFormComponent
      ],
      imports: [
        FormsModule,
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
    fixture = TestBed.createComponent(LoginPanelFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
