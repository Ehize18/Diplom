import { Component, input, signal } from '@angular/core';
import { Button } from "../../../controls/button/button";

export enum IntegrationType {
  VK,
  Telegram,
  Max
}

@Component({
  selector: 'app-integration-panel',
  imports: [Button],
  templateUrl: './integration-panel.html',
  styleUrl: './integration-panel.css',
})
export class IntegrationPanel {
  type = input<IntegrationType>();
  url = signal<string>('');

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
}
