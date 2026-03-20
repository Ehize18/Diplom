import { Component, signal } from '@angular/core';
import { Button } from "../../../controls/button/button";

export interface Good {
  id: string;
  iconUrl: string;
  title: string;
  category: string;
  price: number;
  amount: number;
  orderCount: number;
}

@Component({
  selector: 'app-good-panel',
  imports: [Button],
  templateUrl: './good-panel.html',
  styleUrl: './good-panel.css',
})
export class GoodPanel {
  goodList = signal<Good[]>([]);

  addGood(): void {
    const date = new Date();
    this.goodList.update((goods) => {
      const good: Good = {
        id: date.toLocaleTimeString(),
        iconUrl: 'placeholder.svg',
        title: date.toLocaleTimeString(),
        category: date.toLocaleDateString(),
        price: 1000,
        amount: 100,
        orderCount: 100
      };
      goods.push(good);
      return goods;
    });
  }
}
