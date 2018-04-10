import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginPanelFormComponent } from './login-panel-form.component';

describe(LoginPanelFormComponent.name, () => {
  let component: LoginPanelFormComponent;
  let fixture: ComponentFixture<LoginPanelFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LoginPanelFormComponent ]
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
