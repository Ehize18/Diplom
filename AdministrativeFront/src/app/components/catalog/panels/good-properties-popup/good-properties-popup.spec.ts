import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GoodPropertiesPopup } from './good-properties-popup';

describe('GoodPropertiesPopup', () => {
  let component: GoodPropertiesPopup;
  let fixture: ComponentFixture<GoodPropertiesPopup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GoodPropertiesPopup],
    }).compileComponents();

    fixture = TestBed.createComponent(GoodPropertiesPopup);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
