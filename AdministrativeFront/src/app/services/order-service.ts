import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private _baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this._baseUrl = environment.apiUrl + "/shopcontent";
  }

  updateOrderStatus(
    shopId: string,
    orderId: string,
    entityType: string,
    statusValue: number
  ): Observable<string> {
    const body = {
      entityType: entityType,
      statusValue: statusValue
    };
    return this.httpClient.put<string>(
      `${this._baseUrl}/${shopId}/order/${orderId}`,
      body,
      { withCredentials: true }
    );
  }
}
