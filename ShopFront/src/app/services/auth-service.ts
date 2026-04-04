import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { BasketService } from './basket-service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public userId = signal<string>('');

  constructor(
    private httpClient: HttpClient,
    private basketService: BasketService
  ) {
  }

  async authBase(shopId: string): Promise<boolean> {
    const body = {
      userId: '019d38d0-7a4d-7d6c-9277-c08b450e4d48',
      username: 'TestUser'
    }
    let result = false;
    await this.httpClient.post<boolean>(environment.API_URL + "/auth/base", body, {
      headers: {
        'X-Shop-Id': shopId
      },
      withCredentials: true
    }).forEach(
      (value) => {
        console.log(value);
        result = value;
      }
    );
    this.basketService.loadBasket(shopId);
    return result;
  }

  async authVk(shopId: string): Promise<boolean> {
    const body = {
      firstName: window.sessionStorage.getItem('VkFirstName'),
      lastName: window.sessionStorage.getItem('VkLastName'),
      id: Number.parseInt(window.sessionStorage.getItem('VkId')!),
      sign: window.sessionStorage.getItem('VkSign'),
      ts: Number.parseInt(window.sessionStorage.getItem('VkTs')!)
    }
    let result = false;
    await this.httpClient.post<boolean>(environment.API_URL + "/auth/vk", body, {
      headers: {
        'X-Shop-Id': shopId
      },
      withCredentials: true
    }).forEach(
      (value) => {
        console.log(value);
        result = value;
      }
    );
    this.basketService.loadBasket(shopId);
    return result;
  }

  async auth(shopId: string): Promise<boolean> {
    const authType = window.sessionStorage.getItem('AuthType');
    if (authType === 'vk') {
      return await this.authVk(shopId);
    }
    return await this.authBase(shopId);
  }
}
