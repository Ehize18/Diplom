import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryTreePanel } from './category-tree-panel';

describe('CategoryTreePanel', () => {
  let component: CategoryTreePanel;
  let fixture: ComponentFixture<CategoryTreePanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryTreePanel],
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryTreePanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
