import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LastEventsPanel } from './last-events-panel';

describe('LastEventsPanel', () => {
  let component: LastEventsPanel;
  let fixture: ComponentFixture<LastEventsPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LastEventsPanel],
    }).compileComponents();

    fixture = TestBed.createComponent(LastEventsPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
