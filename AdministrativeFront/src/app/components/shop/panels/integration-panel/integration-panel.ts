import { Component, input, OnInit, signal } from '@angular/core';
import { Button } from "../../../controls/button/button";
import { ShopService } from '../../../../services/shop-service';
import { Shop } from '../../../../contracts/shop';
import { ControlType, Popup, PopupConfig } from '../../../controls/popup/popup';

export enum IntegrationType {
  VK,
  Telegram,
  Max
}

@Component({
  selector: 'app-integration-panel',
  imports: [Button, Popup],
  templateUrl: './integration-panel.html',
  styleUrl: './integration-panel.css',
})
export class IntegrationPanel {
  type = input<IntegrationType>();
  url = signal<string>('Не настроено');
  vkGroupId = signal<number | null>(null);
  isModalOpened = signal<boolean>(false);

  constructor(private shopService: ShopService) {
    this.shopService.getCurrentShopObservable().subscribe(
      shop => {
        if (shop) {
          this.loadData(shop);
        }
      }
    )
  }

  getTitle(): string {
    switch (this.type()) {
      case IntegrationType.VK:
        return 'Вконтакте';
      case IntegrationType.Telegram:
        return 'Telegram';
      case IntegrationType.Max:
        return 'Max';
      default:
        return 'Ошибка';
    }
  }

  getImageSrc(): string {
    switch (this.type()) {
      default:
        return 'placeholder.svg';
    }
  }

  getUrl(): string {
    if (this.url() === '') {
      return 'Не настроено';
    }
    return this.url();
  }

  loadData(shop: Shop): void {
    switch (this.type()) {
      case IntegrationType.VK:
        this.vkGroupId.set(shop.vkGroupId);
        if (shop.vkGroupId) {
          this.url.set(`https://vk.com/club${shop.vkGroupId}`);
        }
        else {
          this.url.set('Не настроено');
        }
    }
  }

  onCloseModal(tag: string, controls: any): void {
    this.isModalOpened.set(false);
    console.log(controls);
    switch (this.type()) {
      case IntegrationType.VK:
        this.shopService.updateVkShop(
          Number.parseInt(controls.id)
        );
    }
  }

  onSettingsButtonClick(): void {
    this.isModalOpened.set(true);
  }

  getId(): string | null | undefined {
    switch (this.type()) {
      case IntegrationType.VK:
        return this.vkGroupId()?.toString();
      default:
        return '';
    }
  }

  getPopupConfig(): PopupConfig {
    return {
      id: 'settings' + this.type()?.toString(),
      controls: [
        {
          type: ControlType.Input,
          tag: 'id',
          caption: 'Id',
          value: this.getId() || '',
          lookupData: []
        }
      ],
      buttons: [
        {
          caption: 'Сохранить',
          tag: 'save'
        },
        {
          caption: 'Закрыть',
          tag: 'close'
        }
      ],
      callback: this.onCloseModal.bind(this)
    }
  }
}
