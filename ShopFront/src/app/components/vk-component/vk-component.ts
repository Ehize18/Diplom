import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import bridge from '@vkontakte/vk-bridge';
import { AuthService } from '../../services/auth-service';
import { ShopService } from '../../services/shop-service';

@Component({
  selector: 'app-vk-component',
  imports: [],
  templateUrl: './vk-component.html',
  styleUrl: './vk-component.css',
})
export class VkComponent implements OnInit {
  constructor(
    private router: Router,
    private authService: AuthService,
    private shopService: ShopService
  ) {

  }

  async ngOnInit(): Promise<void> {
    const launchParams = await bridge.send('VKWebAppGetLaunchParams');
    const userInfo = await bridge.send('VKWebAppGetUserInfo');
    const payload = `${userInfo.id}`;

    window.sessionStorage.setItem('AuthType', 'vk');

    window.sessionStorage.setItem('VkFirstName', userInfo.first_name);
    window.sessionStorage.setItem('VkLastName', userInfo.last_name);
    window.sessionStorage.setItem('VkId', userInfo.id.toString());

    const hash = await bridge.send('VKWebAppCreateHash', {
      payload: payload
    });

    window.sessionStorage.setItem('VkSign', hash.sign);
    window.sessionStorage.setItem('VkTs', hash.ts.toString());
    
    const images = {
      photo_100 : userInfo.photo_100,
      photo_200 : userInfo.photo_200
    }

    window.sessionStorage.setItem('VkImages', JSON.stringify(images));

    if (launchParams.vk_group_id) {
      const shopId = await this.shopService.getShopIdByVk(launchParams.vk_group_id);
      if (shopId) {
        this.router.navigate([shopId]);
      }
    }

    bridge.send('VKWebAppInit');
  }
}