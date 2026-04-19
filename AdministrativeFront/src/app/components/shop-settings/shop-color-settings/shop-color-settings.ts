import { Component, signal, output, AfterViewInit, input, effect } from '@angular/core';

export interface ColorSetting {
  variable: string;
  label: string;
  value: string;
}

@Component({
  selector: 'app-shop-color-settings',
  imports: [],
  templateUrl: './shop-color-settings.html',
  styleUrl: './shop-color-settings.css',
})
export class ShopColorSettings implements AfterViewInit {
  loadedColors = input<ColorSetting[]>([]);
  colors = signal<ColorSetting[]>([
    {
      variable: '--primary-bg-color',
      label: 'Основной фон',
      value: '#CAE9FF',
    },
    {
      variable: '--card-bg-color',
      label: 'Фон карточек',
      value: '#FAFAFA',
    },
    {
      variable: '--primary-accent-color',
      label: 'Основной акцент',
      value: '#93E2FF',
    },
    {
      variable: '--primary-border-color',
      label: 'Цвет бордера',
      value: '#1B4965',
    },
    {
      variable: '--primary-font-color',
      label: 'Цвет текста',
      value: '#000',
    },
    {
      variable: '--primary-font-accent-color',
      label: 'Цвет акцентного текста',
      value: '#000',
    }
  ]);

  constructor() {
    effect(() => {
      const loaded = this.loadedColors();
      if (loaded && loaded.length > 0) {
        this.colors.set(loaded);
        this.colorChange.emit(this.colors());
      }
    });
  }

  ngAfterViewInit(): void {
    this.colorChange.emit(this.colors());
  }

  colorChange = output<ColorSetting[]>();

  onColorChange(setting: ColorSetting, event: Event): void {
    const input = event.target as HTMLInputElement;
    setting.value = input.value;
    this.colorChange.emit(this.colors());
  }
}
