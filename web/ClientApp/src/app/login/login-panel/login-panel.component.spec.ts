import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RouterTestingModule } from '@angular/router/testing';

import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHandler } from '@angular/common/http';

import { MatIconModule, MatMenuModule } from '@angular/material';

import { LoginPanelComponent } from './login-panel.component';
import { LoginPanelFormComponent } from '../login-panel-form/login-panel-form.component';
import { AuthenticationService } from '../../services/authentication.service';


describe('LoginPanelComponent', () => {
  let component: LoginPanelComponent;
  let fixture: ComponentFixture<LoginPanelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        LoginPanelComponent,
        LoginPanelFormComponent
      ],
      imports: [
        FormsModule,
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
    fixture = TestBed.createComponent(LoginPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
