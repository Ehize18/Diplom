import { Component, input, signal } from '@angular/core';
import { Button } from '../../../controls/button/button';
import { Popup, PopupConfig, ControlType, ControlConfig } from '../../../controls/popup/popup';
import { Method } from '../../../../contracts/methods';
import { ShopService } from '../../../../services/shop-service';
import { CatalogService } from '../../../../services/catalog-service';
import { LookupData } from '../../../controls/lookup/lookup';

export enum MethodType {
  Payment,
  Delivery
}

@Component({
  selector: 'app-methods-panel',
  imports: [Button, Popup],
  templateUrl: './methods-panel.html',
  styleUrl: './methods-panel.css',
})
export class MethodsPanel {
  type = input<MethodType>();
  methods = signal<Method[]>([]);
  isModalOpened = signal(false);
  selectedMethod: Method | null = null;

  constructor(
    private shopService: ShopService,
    private catalogService: CatalogService
  ) {
    this.shopService.getCurrentShopObservable().subscribe(
      shop => {
        if (shop) {
          this.loadMethods(shop.id);
        }
      }
    );
  }

  loadMethods(shopId: string): void {
    if (this.type() === MethodType.Payment) {
      this.catalogService.getPaymentMethods(shopId).subscribe(
        response => {
          if (response.isSuccess) {
            this.methods.set(response.results);
          }
        }
      );
    } else {
      this.catalogService.getDeliveryMethods(shopId).subscribe(
        response => {
          if (response.isSuccess) {
            this.methods.set(response.results);
          }
        }
      );
    }
  }

  selectMethod(method: Method): void {
    this.selectedMethod = method;
    this.isModalOpened.set(true);
  }

  getOnAddClickCallback(): Function {
    return this.onAddClick.bind(this);
  }

  onAddClick(): void {
    this.selectedMethod = null;
    this.isModalOpened.set(true);
  }

  onCloseModal(tag: string, controls: any): void {
    this.isModalOpened.set(false);

    if (tag !== 'save') {
      return;
    }

    const shopId = this.shopService.currentShop?.id;
    if (!shopId) {
      return;
    }

    const title = controls.title;
    if (!title) {
      return;
    }

    if (this.selectedMethod) {
      // Редактирование — собираем метаданные и обновляем
      const metadata = this.buildMetadata(controls);
      if (this.type() === MethodType.Payment) {
        this.catalogService.updatePaymentMethod(shopId, this.selectedMethod.id, title, metadata).subscribe(
          () => this.loadMethods(shopId)
        );
      } else {
        this.catalogService.updateDeliveryMethod(shopId, this.selectedMethod.id, title, metadata).subscribe(
          () => this.loadMethods(shopId)
        );
      }
    } else {
      // Создание нового метода
      const methodTypeId = controls.methodType?.id;
      if (!methodTypeId) {
        return;
      }

      const metadata = this.buildMetadata(controls);

      if (this.type() === MethodType.Payment) {
        const paymentType = methodTypeId === 'OnDelivered' ? 0 : 1;
        this.catalogService.createPaymentMethod(shopId, paymentType, title, metadata).subscribe(
          () => this.loadMethods(shopId)
        );
      } else {
        const deliveryType = methodTypeId === 'Pickup' ? 1 : 2;
        this.catalogService.createDeliveryMethod(shopId, deliveryType, title, metadata).subscribe(
          () => this.loadMethods(shopId)
        );
      }
    }
  }

  buildMetadata(controls: any): Record<string, string> {
    const meta: Record<string, string> = {};

    if (this.type() === MethodType.Payment) {
      meta['payment_info_needed'] = controls.paymentInfoNeeded ? 'true' : 'false';
    } else {
      // Delivery
      const isPickup = this.selectedMethod
        ? (this.selectedMethod.metadata?.['addresses'] !== undefined)
        : (controls.methodType?.id === 'Pickup');

      if (isPickup) {
        const addresses = Array.isArray(controls.addresses)
          ? controls.addresses.filter((a: string) => a.trim()).join(';')
          : '';
        meta['addresses'] = addresses;
      } else {
        meta['address_needed'] = controls.addressNeeded ? 'true' : 'false';
      }
    }

    return meta;
  }

  getMethodTypeLookupData(): LookupData[] {
    if (this.type() === MethodType.Payment) {
      return [
        { id: 'OnDelivered', caption: 'При получении' },
        { id: 'Transfer', caption: 'Перевод' }
      ];
    }
    return [
      { id: 'Pickup', caption: 'Самовывоз' },
      { id: 'Post', caption: 'Почта' }
    ];
  }

  getPopupConfig(): PopupConfig {
    const isEdit = this.selectedMethod !== null;
    const controls: ControlConfig[] = [];

    if (!isEdit) {
      controls.push({
        type: ControlType.Lookup,
        tag: 'methodType',
        caption: 'Тип',
        value: null,
        lookupData: this.getMethodTypeLookupData()
      });
    }

    controls.push({
      type: ControlType.Input,
      tag: 'title',
      caption: 'Название',
      value: isEdit ? this.selectedMethod!.title : '',
      lookupData: []
    });

    // Метаданные
    if (this.type() === MethodType.Payment) {
      const existingMeta = this.selectedMethod?.metadata || {};
      controls.push({
        type: ControlType.Boolean,
        tag: 'paymentInfoNeeded',
        caption: 'Требуются платёжные данные',
        value: existingMeta['payment_info_needed'] === 'true',
        lookupData: [],
        isVisible: (cv: any) => true
      });
    } else {
      // Delivery
      if (isEdit) {
        const existingMeta = this.selectedMethod?.metadata || {};
        if (existingMeta['addresses'] !== undefined) {
          const addresses = existingMeta['addresses']
            ? existingMeta['addresses'].split(';').filter((a: string) => a)
            : [];
          controls.push({
            type: ControlType.List,
            tag: 'addresses',
            caption: 'Адреса',
            value: addresses,
            lookupData: []
          });
        } else {
          controls.push({
            type: ControlType.Boolean,
            tag: 'addressNeeded',
            caption: 'Требуется адрес доставки',
            value: existingMeta['address_needed'] === 'true',
            lookupData: []
          });
        }
      } else {
        controls.push({
          type: ControlType.Boolean,
          tag: 'addressNeeded',
          caption: 'Требуется адрес доставки',
          value: false,
          lookupData: [],
          isVisible: (cv: any) => cv['methodType']?.id === 'Post'
        });
        controls.push({
          type: ControlType.List,
          tag: 'addresses',
          caption: 'Адреса',
          value: [],
          lookupData: [],
          isVisible: (cv: any) => cv['methodType']?.id === 'Pickup'
        });
      }
    }

    return {
      id: 'methodPopup',
      controls: controls,
      buttons: [
        { caption: 'Сохранить', tag: 'save' },
        { caption: 'Закрыть', tag: 'close' }
      ],
      callback: this.onCloseModal.bind(this)
    };
  }

  getImageSrc(): string {
    switch (this.type()) {
      case MethodType.Payment: return "Payment.svg";
      case MethodType.Delivery: return "Delivery.svg";
      default: return "placeholder.svg";
    }
  }

  getTitle(): string {
    switch (this.type()) {
      case MethodType.Payment: return "Способы оплаты";
      case MethodType.Delivery: return "Способы доставки";
      default: return "Ошибка";
    }
  }
}
