import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VkComponent } from './vk-component';

describe('VkComponent', () => {
  let component: VkComponent;
  let fixture: ComponentFixture<VkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VkComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(VkComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
