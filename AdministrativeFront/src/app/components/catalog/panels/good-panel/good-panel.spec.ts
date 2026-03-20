import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GoodPanel } from './good-panel';

describe('GoodPanel', () => {
  let component: GoodPanel;
  let fixture: ComponentFixture<GoodPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GoodPanel],
    }).compileComponents();

    fixture = TestBed.createComponent(GoodPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
