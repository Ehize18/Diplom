import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateShopRequest, Shop } from '../contracts/shop';

@Injectable({
  providedIn: 'root',
})
export class ShopService {

  get currentShop() {
    const currentShopString = window.localStorage.getItem("CurrentShop");
    if (currentShopString) {
      const currentShop: Shop = JSON.parse(currentShopString);
      return currentShop;
    }
    return undefined;
  }

  set currentShop(shop: Shop | undefined) {
    if (shop) {
      const currentShopString = JSON.stringify(shop);
      window.localStorage.setItem("CurrentShop", currentShopString);
    }
  }

  private baseUrl: string;

  private HTTP_OPTIONS = {
    withCredentials: true
  };

  constructor(private httpClient: HttpClient) {
    this.baseUrl = environment.apiUrl + "/shop"
  }

  public getUserShops(): Observable<Shop[]> {
    return this.httpClient.get<Shop[]>(this.baseUrl, this.HTTP_OPTIONS);
  }

  public createShop(createShopRequest : CreateShopRequest): Observable<Shop> {
    return this.httpClient.post<Shop>(this.baseUrl, createShopRequest, this.HTTP_OPTIONS);
  }
}
