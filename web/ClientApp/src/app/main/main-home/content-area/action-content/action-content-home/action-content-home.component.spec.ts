import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActionContentHomeComponent } from './action-content-home.component';

describe('ActionContentHomeComponent', () => {
  let component: ActionContentHomeComponent;
  let fixture: ComponentFixture<ActionContentHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActionContentHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActionContentHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
