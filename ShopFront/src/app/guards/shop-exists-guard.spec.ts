import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { shopExistsGuard } from './shop-exists-guard';

describe('shopExistsGuardGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => shopExistsGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
