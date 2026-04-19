import { Component, ElementRef, OnInit, signal } from '@angular/core';
import { ShopCardPreview } from '../shop-card-preview/shop-card-preview';
import { ShopColorSettings, ColorSetting } from '../shop-color-settings/shop-color-settings';
import { ShopService } from '../../../services/shop-service';

@Component({
  selector: 'app-shop-settings-component',
  imports: [ShopCardPreview, ShopColorSettings],
  templateUrl: './shop-settings-component.html',
  styleUrl: './shop-settings-component.css',
})
export class ShopSettingsComponent implements OnInit {
  private currentColors = signal<ColorSetting[]>([]);
  loadedColors = signal<ColorSetting[]>([]);
  private isSaving = false;
  private isColorsLoaded = false;

  constructor(
    private el: ElementRef,
    private shopService: ShopService
  ) {}

  ngOnInit(): void {
    this.loadColors();
  }

  onColorsChange(colors: ColorSetting[]): void {
    this.currentColors.set(colors);
    this.applyColors();
    this.saveColors(colors);
  }

  onPreviewInit(): void {
    this.applyColors();
  }

  private loadColors(): void {
    const shopId = this.shopService.currentShop?.id;
    if (!shopId) return;

    this.shopService.loadColors(shopId).subscribe({
      next: (colors) => {
        if (colors && colors.length > 0) {
          this.loadedColors.set(colors);
        }
        this.isColorsLoaded = true;
      },
      error: () => {
        this.isColorsLoaded = true;
      }
    });
  }

  private saveColors(colors: ColorSetting[]): void {
    if (this.isSaving || !this.isColorsLoaded) return;
    this.isSaving = true;

    const shopId = this.shopService.currentShop?.id;
    if (!shopId) {
      this.isSaving = false;
      return;
    }

    this.shopService.saveColors(shopId, colors).subscribe({
      next: () => {
        this.isSaving = false;
      },
      error: () => {
        this.isSaving = false;
      }
    });
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
