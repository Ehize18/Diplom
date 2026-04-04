import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ShopService } from '../services/shop-service';
import { AuthService } from '../services/auth-service';

export const shopExistsGuard: CanActivateFn = async (route, state) => {
  const router = inject(Router);
  const shopService = inject(ShopService);
  const authService = inject(AuthService);

  const shopUuid = route.paramMap.get('shopUuid');

  if (!shopUuid) {
    return router.createUrlTree(['']);
  }

  const isShopExists = await shopService.checkShopExists(shopUuid);

  if (!isShopExists) {
    return router.createUrlTree(['']);
  }

  shopService.setShopId(shopUuid);
  const isAuth = await authService.auth(shopUuid);

  if (!isAuth) {
    return router.createUrlTree(['']);
  }

  return true;
};
