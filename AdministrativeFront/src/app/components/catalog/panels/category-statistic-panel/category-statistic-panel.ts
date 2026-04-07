import { Component, effect, input, signal } from '@angular/core';

export enum CategoryStaticsticType {
  GoodCount,
  GoodCountFull,
  OrderCount,
  OrderCountFull
}

@Component({
  selector: 'app-category-statistic-panel',
  imports: [],
  templateUrl: './category-statistic-panel.html',
  styleUrl: './category-statistic-panel.css',
})
export class CategoryStatisticPanel {
  type = input<CategoryStaticsticType>();
  statistic = input(0);

  getImageSrc(): string {
    switch (this.type()) {
      case CategoryStaticsticType.GoodCount:
        return 'GoodsIcon.svg';
      case CategoryStaticsticType.GoodCountFull:
        return 'GoodsIcon.svg';
      case CategoryStaticsticType.OrderCountFull:
        return 'OrdersIcon.svg';
      default:
        return 'placeholder.svg';
    }
  }

  getTitleCaption(): string {
    switch (this.type()) {
      case CategoryStaticsticType.GoodCount:
        return 'Количество товаров';
      case CategoryStaticsticType.GoodCountFull:
        return 'Количество товаров с подкатегориями';
      case CategoryStaticsticType.OrderCount:
        return 'Количество продаж';
      case CategoryStaticsticType.OrderCountFull:
        return 'Количество продаж с подкатегориями';
      default:
        return 'Ошибка';
    }
  }
}
