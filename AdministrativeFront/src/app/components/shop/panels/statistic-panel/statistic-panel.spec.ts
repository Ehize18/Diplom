import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatisticPanel } from './statistic-panel';

describe('StatisticPanel', () => {
  let component: StatisticPanel;
  let fixture: ComponentFixture<StatisticPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatisticPanel],
    }).compileComponents();

    fixture = TestBed.createComponent(StatisticPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
