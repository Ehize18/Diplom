import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ShopService } from './shop-service';
import { toObservable } from '@angular/core/rxjs-interop';
import { Observable } from 'rxjs';
import { Method, Order } from '../models/order';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  shopId = '';

  get MethodsURL() {
    return environment.API_URL + '/methods';
  }

  get OrdersURL() {
    return environment.API_URL + '/order';
  }

  constructor(
    private httpClient: HttpClient,
    private shopService: ShopService
  ) {
    toObservable(this.shopService.shopId).subscribe(
      (shopId) => {
        this.shopId = shopId
      }
    );
  }

  getPaymentMethods(): Observable<Method[]> {
    return this.httpClient.get<Method[]>(this.MethodsURL + '/payment', {
      headers: {
        'X-Shop-Id': this.shopId
      },
      withCredentials: true
    });
  }

  getDeliveryMethods(): Observable<Method[]> {
    return this.httpClient.get<Method[]>(this.MethodsURL + '/delivery', {
      headers: {
        'X-Shop-Id': this.shopId
      },
      withCredentials: true
    });
  }

  createOrder(
    paymentMethodId: string,
    deliveryMethodId: string,
    deliveryExtras: string,
  ): Observable<any> {
    const body = {
      paymentMethodId: paymentMethodId,
      deliveryMethodId: deliveryMethodId,
      deliveryExtras: deliveryExtras
    }
    return this.httpClient.post(this.OrdersURL, body, {
      headers: {
        'X-Shop-Id': this.shopId
      },
      withCredentials: true
    })
  }

  getOrders(): Observable<Order[]> {
    return this.httpClient.get<Order[]>(this.OrdersURL, {
      headers: {
        'X-Shop-Id': this.shopId
      },
      withCredentials: true
    });
  }

  getOrderById(orderId: string, withBasket: boolean = false): Observable<Order> {
    return this.httpClient.get<Order>(this.OrdersURL + `/${orderId}`, {
      headers: {
        'X-Shop-Id': this.shopId
      },
      withCredentials: true,
      params: {
        'withBasket': withBasket
      }
    });
  }
}
