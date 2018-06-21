import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExcelComponent } from './excel.component';

import { HotTableModule, HotTableRegisterer } from '@handsontable/angular';

xdescribe('ExcelComponent', () => {
  let component: ExcelComponent;
  let fixture: ComponentFixture<ExcelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExcelComponent ],
      imports: [
        HotTableModule
      ],
      providers: [
        HotTableRegisterer
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExcelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
