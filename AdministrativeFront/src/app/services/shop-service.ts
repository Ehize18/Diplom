import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateShopRequest, Shop } from '../contracts/shop';
import { toObservable } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class ShopService {
  private _currentShop = signal<Shop | undefined>(undefined);

  get currentShop() {
    let currentShop : Shop | undefined = this._currentShop();
    if (currentShop) {
      return currentShop;
    }
    const currentShopString = window.localStorage.getItem("CurrentShop");
    if (currentShopString) {
      currentShop = JSON.parse(currentShopString);
      return currentShop;
    }
    return undefined;
  }

  set currentShop(shop: Shop | undefined) {
    if (shop) {
      const currentShopString = JSON.stringify(shop);
      window.localStorage.setItem("CurrentShop", currentShopString);
      this._currentShop.set(shop);
    }
  }

  private baseUrl: string;

  private HTTP_OPTIONS = {
    withCredentials: true
  };

  constructor(private httpClient: HttpClient) {
    this.baseUrl = environment.apiUrl + "/shop";
    if (!this._currentShop()) {
      if (this.currentShop) {
        this._currentShop.set(this.currentShop);
      }
      else {
        this.getUserShops().subscribe((
          (value) => {
            if (value[0]) {
              this.currentShop = value[0];
            }
          }
        ));
      }
    }
  }

  public getCurrentShopObservable(): Observable<Shop | undefined> {
    return toObservable(this._currentShop);
  }

  public getUserShops(): Observable<Shop[]> {
    return this.httpClient.get<Shop[]>(this.baseUrl, this.HTTP_OPTIONS);
  }

  public createShop(createShopRequest : CreateShopRequest): Observable<Shop> {
    return this.httpClient.post<Shop>(this.baseUrl, createShopRequest, this.HTTP_OPTIONS);
  }
}
