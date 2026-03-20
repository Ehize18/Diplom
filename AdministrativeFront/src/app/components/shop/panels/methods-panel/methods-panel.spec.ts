import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MethodsPanel } from './methods-panel';

describe('MethodsPanel', () => {
  let component: MethodsPanel;
  let fixture: ComponentFixture<MethodsPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MethodsPanel],
    }).compileComponents();

    fixture = TestBed.createComponent(MethodsPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
