import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActionContentComponent } from './action-content.component';

describe('ActionContentComponent', () => {
  let component: ActionContentComponent;
  let fixture: ComponentFixture<ActionContentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActionContentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActionContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
