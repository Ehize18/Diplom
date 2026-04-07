import { Component, input, signal } from '@angular/core';
import { Button } from '../../../controls/button/button';
import { Method } from '../../../../contracts/methods';

export enum MethodType {
  Payment,
  Delivery
}

@Component({
  selector: 'app-methods-panel',
  imports: [Button],
  templateUrl: './methods-panel.html',
  styleUrl: './methods-panel.css',
})
export class MethodsPanel {
  type = input<MethodType>();
  methods = signal<Method[]>([]);

  getImageSrc(): string {
    switch (this.type()) {
      case MethodType.Payment: 
        return "Payment.svg";
      case MethodType.Delivery:
        return "Delivery.svg";
      default:
        return "placeholder.svg";
    }
  }

  getTitle(): string {
    switch (this.type()) {
      case MethodType.Payment: 
        return "Способы оплаты";
      case MethodType.Delivery:
        return "Способы доставки";
      default:
        return "Ошибка";
    }
  }

  getOnAddClickCallback(): Function {
    return this.onAddClick.bind(this);
  }

  onAddClick(): void {
    this.methods.update(methods => {
      methods.push(this.getNewMethod());
      return methods;
    });
  }

  private getNewMethod(): Method {
    const date = new Date();
    switch (this.type()) {
      default:
        return {
          id: date.getTime().toString(),
          title: date.toLocaleTimeString(),
          metadata: {}
        }
    }
  }
}
