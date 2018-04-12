import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MatIconModule, MatMenuModule } from '@angular/material';

import { LoginPanelComponent } from './login-panel.component';
import { LoginPanelFormComponent } from '../login-panel-form/login-panel-form.component';

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
        MatIconModule,
        MatMenuModule
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
