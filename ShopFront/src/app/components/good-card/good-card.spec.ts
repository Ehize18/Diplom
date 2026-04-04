import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GoodCard } from './good-card';

describe('GoodCard', () => {
  let component: GoodCard;
  let fixture: ComponentFixture<GoodCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GoodCard],
    }).compileComponents();

    fixture = TestBed.createComponent(GoodCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
