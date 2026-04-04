import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AddGoodToBasketRequest, Basket, BasketItem } from '../models/basket';

@Injectable({
  providedIn: 'root',
})
export class BasketService {
  currentBasket = signal<Basket>({ items: []});

  get URL() {
    return environment.API_URL + '/basket';
  }

  constructor(private httpClient: HttpClient) {
  }

  addGoodToBasket(shopId: string, goodId: string, count: number): Observable<any> {
    const body : AddGoodToBasketRequest =  {
      goodId: goodId,
      count: count
    }
    return this.httpClient.post(this.URL + '/good', body, {
      headers: {
        'X-Shop-Id': shopId
      },
      withCredentials: true
    });
  }

  async loadBasket(shopId: string): Promise<void> {
    return this.httpClient.get<Basket>(this.URL, {
      headers: {
        'X-Shop-Id': shopId
      },
      withCredentials: true
    }).forEach(
      basket => {
        this.currentBasket.set(basket);
      }
    )
  }

  changeBasketItemCount(shopId: string, item: BasketItem): Observable<number> {
    const body =  {
      count: item.count
    };
    return this.httpClient.patch<number>(this.URL + `/item/${item.basketItemId}`, body, {
      headers: {
        'X-Shop-Id': shopId
      },
      withCredentials: true
    });
  }

  deleteBasketItem(shopId: string, item: BasketItem): void {
    this.httpClient.delete<number>(this.URL + `/item/${item.basketItemId}`, {
      headers: {
        'X-Shop-Id': shopId
      },
      withCredentials: true
    }).subscribe(
      count => {
        if (count < 0) {
          console.error('Count error');
        }
        else {
          this.currentBasket.update(
            basket => {
              const newItems = basket.items.filter(x => x.basketItemId !== item.basketItemId);
              return { items: newItems };
            });
        }
      }
    );
  }
}
