import { Component, ElementRef, signal } from '@angular/core';
import { ShopCardPreview } from '../shop-card-preview/shop-card-preview';
import { ShopColorSettings, ColorSetting } from '../shop-color-settings/shop-color-settings';

@Component({
  selector: 'app-shop-settings-component',
  imports: [ShopCardPreview, ShopColorSettings],
  templateUrl: './shop-settings-component.html',
  styleUrl: './shop-settings-component.css',
})
export class ShopSettingsComponent {
  private currentColors = signal<ColorSetting[]>([]);
  private previewInited = false;

  constructor(private el: ElementRef) {}

  onColorsChange(colors: ColorSetting[]): void {
    this.currentColors.set(colors);
    this.applyColors();
  }

  onPreviewInit(): void {
    this.previewInited = true;
    this.applyColors();
  }

  private applyColors(): void {
    const colors = this.currentColors();
    const previewFrame = this.el.nativeElement.querySelector('.preview-frame') as HTMLElement | null;
    if (!previewFrame) return;

    colors.forEach((c) => {
      previewFrame.style.setProperty(`${c.variable}`, c.value);
    });
  }
}
