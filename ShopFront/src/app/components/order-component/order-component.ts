import { Component, computed, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BasketItem } from '../../models/basket';
import { BasketService } from '../../services/basket-service';
import { ShopService } from '../../services/shop-service';
import { environment } from '../../../environments/environment';
import { Method } from '../../models/order';
import { OrderService } from '../../services/order-service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-order-component',
  imports: [FormsModule],
  templateUrl: './order-component.html',
  styleUrl: './order-component.css',
})
export class OrderComponent implements OnInit {
  isEditable = signal<boolean>(false);
  id = '';
  caption = computed(() => {
    if (this.isEditable()) {
      return 'Новый заказ';
    }
    return `Заказ №${this.id}`;
  });
  basketItems = signal<BasketItem[]>([]);
  paymentMethods = signal<Method[]>([]);
  deliveryMethods = signal<Method[]>([]);
  selectedPaymentMethod = signal<Method | null>(null);
  selectedDeliveryMethod = signal<Method | null>(null);
  isDeliveryExtrasVisible = signal<boolean>(false);
  deliveryExtras = signal<string>('');
  deliveryAddresses = signal<string[]>([]);

  totalPrice = computed(() => {
    let total = 0;
    this.basketItems().forEach(item => {
      total += item.price * item.count
    });
    return total;
  })

  isValid = computed(() => {
    if (!this.selectedDeliveryMethod()) {
      return false;
    }
    if (this.deliveryExtras().length < 1) {
      return false;
    }
    if (!this.selectedPaymentMethod()) {
      return false;
    }
    return true;
  });

  constructor(
    private route: ActivatedRoute,
    private basketService: BasketService,
    private shopService: ShopService,
    private orderService: OrderService
  ) {
  }

  async ngOnInit(): Promise<void> {
    const isNew = this.route.snapshot.queryParamMap.get('isNew');
    this.id = this.route.snapshot.paramMap.get('orderId') || '';
    this.isEditable.set(isNew === 'true');
    await this.loadMethods();
    if (this.isEditable()) {
      let basket = this.basketService.currentBasket();
      if (basket.items.length === 0) {
        await this.basketService.loadBasket(
          this.shopService.shopId()
        );
        basket = this.basketService.currentBasket();
      }
      this.basketItems.set(basket.items);
    }
    if (this.id != '') {
      this.orderService.getOrderById(this.id, true).subscribe(
        order => {
          console.log(order);
          const paymentMethod = this.paymentMethods().find(x => x.id === order.paymentMethodId);
          const deliveryMethod = this.deliveryMethods().find(x => x.id === order.deliveryMethodId);
          if (paymentMethod) {
            this.selectedPaymentMethod.set(paymentMethod);
          }
          if (deliveryMethod) {
            this.selectedDeliveryMethod.set(deliveryMethod);
            this.onDeliveryMethodChanged();
          }
          this.deliveryExtras.set(order.deliveryExtras);
          this.basketItems.set(order.basket.items);
        }
      )
    }
  }

  loadMethods(): Promise<void> {
    return new Promise(resolve => {
      let isPaymentLoaded = false;
      let isDeliveryLoaded = false;
      this.orderService.getPaymentMethods().subscribe(
        methods => {
          this.paymentMethods.set(methods);
          if (isDeliveryLoaded) {
            resolve();
          } else {
            isPaymentLoaded = true;
          }
        }
      );
      this.orderService.getDeliveryMethods().subscribe(
        methods => {
          this.deliveryMethods.set(methods);
          if (isPaymentLoaded) {
            resolve();
          } else {
            isDeliveryLoaded = true;
          }
        }
      );
    });
  }

  getImageSrc(basketItem: BasketItem): string {
    if (basketItem.imageId) {
      return `${environment.IMAGES_URL}/${this.shopService.shopId()}/${basketItem.imageId}`;
    }
    return 'placeholder.svg';
  }

  onDeliveryMethodChanged() {
    const method = this.selectedDeliveryMethod();
    this.deliveryExtras.set('');
    this.isDeliveryExtrasVisible.set(false);
    if (!method) {
      return;
    }
    if (method.metadata['address_needed'] === 'true') {
      this.isDeliveryExtrasVisible.set(true);
    }
    if (method.metadata['addresses'] && method.metadata['addresses'].length > 0) {
      const addresses = method.metadata['addresses'].split(';');
      this.deliveryAddresses.set(addresses);
      this.isDeliveryExtrasVisible.set(true);
    }
  }

  getDeliveryExtrasType(): string {
    const method = this.selectedDeliveryMethod()!;
    if (method.metadata['address_needed'] === 'true') {
      return 'address_input';
    }
    if (method.metadata['addresses']) {
      return 'addresses';
    }
    return '';
  }

  onConfirmOrderButtonClick(): void {
    console.log('order');
    const paymentMethod = this.selectedPaymentMethod();
    const deliveryMethod = this.selectedDeliveryMethod();

    if (!paymentMethod || !deliveryMethod) {
      return;
    }

    this.orderService.createOrder(
      paymentMethod.id,
      deliveryMethod.id,
      this.deliveryExtras()
    ).subscribe(
      value => console.log(value)
    );
  }
}
