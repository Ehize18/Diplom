import { Component, OnInit, signal } from '@angular/core';
import { RouterOutlet, RouterLinkWithHref, Router, NavigationEnd } from '@angular/router';
import { filter, map, Observable } from 'rxjs';
import { AuthService } from '../../../services/auth-service';
import { ShopService } from '../../../services/shop-service';
import { CreateShopRequest, Shop } from '../../../contracts/shop';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-shop-layout-component',
  imports: [RouterOutlet, RouterLinkWithHref, FormsModule],
  templateUrl: './shop-layout-component.html',
  styleUrl: './shop-layout-component.css',
})
export class ShopLayoutComponent implements OnInit {
  userName = '';
  isDropdownOpen = false;
  isShopsDropdownOpen = false;
  userShops = signal<Shop[]>([]);
  currentShop = signal<Shop | undefined>(undefined);
  isAddShopFormVisible = signal(false);
  newShop: Shop;
  currentPath = '';

  constructor(
    private router: Router,
    private authService: AuthService,
    private shopService: ShopService
  ) {
    this.currentPath = this.router.url;

    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.currentPath = event.urlAfterRedirects
      });
    
    this.newShop = this.initNewShop();
  }

  initNewShop(): Shop {
    this.newShop = { id: '', title: '', description: '', vkGroupId: null, admins: [] };
    return this.newShop;
  }

  ngOnInit(): void {
    const username = this.authService.user?.username;

    if (username) {
      this.userName = username;
    }

    const currentShop = this.shopService.currentShop;

    if (currentShop) {
      this.currentShop.set(currentShop);
    }
    else {
      this.loadUserShops().subscribe({
        next: (shops) => {
          this.currentShop.set(shops[0]);
        }
      });
    }
  }

  loadUserShops(): Observable<Shop[]> {
    return this.shopService.getUserShops().pipe(
      map(shops => {
        this.userShops.set(shops);
        return shops;
      })
    );
  } 

  toggleDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  onSettingsClick(): void {
    console.log('Settings clicked');
  }

  onNavItemClick(path: string): void {
    this.router.navigate([path]);
  }

  isNavItemActive(path: string): boolean {
    return this.currentPath === path;
  }

  onAddShopButtonClick(): void {
    
    this.isAddShopFormVisible.set(true);
  }

  onSubmitAddShop(): void {
    this.shopService.createShop(this.newShop).subscribe({
      next: (shop) => {
        this.userShops.update(shops => {
          const newShops = [...shops];
          newShops.push(shop);
          return newShops
        });
      },
    });
    this.initNewShop();
    this.isAddShopFormVisible.set(false);
  }

  onShopsDropdownClick(): void {
    this.isShopsDropdownOpen = !this.isShopsDropdownOpen;

    if (this.userShops.length === 0) {
      this.loadUserShops().subscribe();
    }
  }

  onChangeCurrentShopClick(shop: Shop): void {
    this.currentShop.set(shop);
    this.shopService.currentShop = shop;
    this.isShopsDropdownOpen = false;
  }
}
