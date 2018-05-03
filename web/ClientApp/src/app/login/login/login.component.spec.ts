import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHandler } from '@angular/common/http';

import { RouterTestingModule } from '@angular/router/testing';

import { LoginComponent } from './login.component';
import { LoginPanelComponent } from '../login-panel/login-panel.component';
import { LoginPanelFormComponent } from '../login-panel-form/login-panel-form.component';
import { AuthenticationService } from '../../services/authentication.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        LoginComponent,
        LoginPanelComponent,
        LoginPanelFormComponent
      ],
      imports: [
        FormsModule,
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
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
