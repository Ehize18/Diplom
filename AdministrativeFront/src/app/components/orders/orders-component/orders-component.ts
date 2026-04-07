import { Component, OnInit, signal } from '@angular/core';
import { Button } from '../../controls/button/button';
import { ShopService } from '../../../services/shop-service';
import { CatalogService } from '../../../services/catalog-service';
import { Order, OrderStatus } from '../../../contracts/order';
import { environment } from '../../../../environments/environment';

class OrderVM {
  model: Order
  isNeedExpand: boolean;
  isExpanded = signal<boolean>(false);
  goodImageSrc = new Map<string, string>();

  constructor(order: Order, shopId: string) {
    this.model = order;
    this.isNeedExpand = order.basket.goods.length > 1;
    order.basket.goods.forEach(item => {
      if (item.good.imageId) {
        this.goodImageSrc.set(item.id, `${environment.imageUrl}/${shopId}/${item.good.imageId}`);
      } else {
        this.goodImageSrc.set(item.id, 'placeholder.svg');
      }
    });
  }

  toggleExpand(): void {
    this.isExpanded.set(!this.isExpanded());
  }
}

@Component({
  selector: 'app-orders-component',
  imports: [Button],
  templateUrl: './orders-component.html',
  styleUrl: './orders-component.css',
})
export class OrdersComponent implements OnInit {
  orders = signal<OrderVM[]>([]);


  constructor(
    private shopService: ShopService,
    private catalogService: CatalogService
  ) {
    this.shopService.getCurrentShopObservable().subscribe(
      shop => {
        this.catalogService.getOrders(shop!.id).subscribe(
          response => {
            if (response.isSuccess) {
              const arr: OrderVM[] = []
              response.results.forEach(order => {
                order.createdAt = new Date(order.createdAt);
                arr.push(new OrderVM(order, shop!.id));
              });
              this.orders.set(arr);
            }
          }
        )
      }
    )
  }

  ngOnInit(): void {
    
  }

  parseDateTime(datetime: Date): string {
    const time = datetime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    const date = datetime.toLocaleDateString();
    return `${time} ${date}`;
  }

  orderStatusLocale(orderStatus: OrderStatus): string {
    switch (orderStatus) {
      case OrderStatus.Created:
        return 'Создан';
      case OrderStatus.Payment:
        return 'Ждёт оплаты';
      case OrderStatus.Delivery:
        return 'В доставке';
      case OrderStatus.Completed:
        return 'Завершён';
      case OrderStatus.Canceled:
        return 'Отменён';
    }
  }

  getImageSrc(order: OrderVM, itemId: string): string {
    return order.goodImageSrc.get(itemId) || 'placeholder.svg';
  }
}
