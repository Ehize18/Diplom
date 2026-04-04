import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GoodPropertiesComponent } from './good-properties-component';

describe('GoodPropertiesComponent', () => {
  let component: GoodPropertiesComponent;
  let fixture: ComponentFixture<GoodPropertiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GoodPropertiesComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GoodPropertiesComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
