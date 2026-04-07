import { Component, input, signal } from '@angular/core';

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
  statistic = signal(0);

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
