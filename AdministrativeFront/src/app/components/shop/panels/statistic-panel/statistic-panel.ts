import { Component, effect, input, signal } from '@angular/core';
import { ShopStatistics } from '../../../../contracts/statistics';

export enum StatisticType {
  Clients,
  Orders,
  Categories,
  Goods,
  Chats
}

@Component({
  selector: 'app-statistic-panel',
  imports: [],
  templateUrl: './statistic-panel.html',
  styleUrl: './statistic-panel.css',
})
export class StatisticPanel {
  type = input<StatisticType>();
  statistics = input<ShopStatistics | null>(null);
  statistic = signal(0);

  constructor() {
    effect(() => {
      const stats = this.statistics();
      if (!stats) {
        return;
      }
      switch (this.type()) {
        case StatisticType.Clients:
          this.statistic.set(stats.clientsCount);
          break;
        case StatisticType.Orders:
          this.statistic.set(stats.ordersCount);
          break;
        case StatisticType.Categories:
          this.statistic.set(stats.categoriesCount);
          break;
        case StatisticType.Goods:
          this.statistic.set(stats.goodsCount);
          break;
        case StatisticType.Chats:
          this.statistic.set(0);
          break;
      }
    });
  }

  getImageSrc(): string {
    switch (this.type()) {
      case StatisticType.Clients:
        return 'ClientsIcon.svg';
      case StatisticType.Orders:
        return 'OrdersIcon.svg';
      case StatisticType.Categories:
        return 'CategoriesIcon.svg';
      case StatisticType.Goods:
        return 'GoodsIcon.svg';
      case StatisticType.Chats:
        return 'ChatsIcon.svg';
      default: return "placeholder.svg"
    }
  }

  getTitleCaption(): string {
    switch (this.type()) {
      case StatisticType.Clients:
        return "Клиенты";
      case StatisticType.Orders:
        return "Заказы";
      case StatisticType.Categories:
        return "Категории";
      case StatisticType.Chats:
        return "Чаты";
      case StatisticType.Goods:
        return "Товары"
      default:
        return "Неизвестный тип"
    }
  }
}
