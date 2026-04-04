import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { environment } from '../../environments/environment';

interface ShopByVkResponse {
  shopId: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class ShopService {
  public shopId = signal<string>('');

  constructor(private httpClient: HttpClient) {
  }

  setShopId(shopId: string): void {
    this.shopId.set(shopId);
    console.log(shopId);
  }

  async checkShopExists(shopId: string): Promise<boolean> {
    let result = false;
    await this.httpClient.get<boolean>(environment.API_URL + "/administration/checkshop", {
      headers: {
        'X-Shop-Id': shopId
      }
    }).forEach(
      (value) => {
        console.log(value);
        result = value;
      }
    );
    return result;
  }

  async getShopIdByVk(vkGroupId: number): Promise<string | null> {
    const params = {
      vkGroupId: vkGroupId
    };
    let result = null;
    await this.httpClient.get<ShopByVkResponse>(environment.API_URL + '/administration/vk', {
      params: params
    }).forEach(
      (value) => {
        result = value.shopId;
      }
    );
    return result;
  }
}
