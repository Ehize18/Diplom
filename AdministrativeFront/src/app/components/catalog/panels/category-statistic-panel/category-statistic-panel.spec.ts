import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryStatisticPanel } from './category-statistic-panel';

describe('CategoryStatisticPanel', () => {
  let component: CategoryStatisticPanel;
  let fixture: ComponentFixture<CategoryStatisticPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryStatisticPanel],
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryStatisticPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
