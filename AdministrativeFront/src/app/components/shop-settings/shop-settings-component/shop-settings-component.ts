import { Component, ElementRef, OnInit, signal } from '@angular/core';
import { ShopCardPreview } from '../shop-card-preview/shop-card-preview';
import { ShopColorSettings, ColorSetting } from '../shop-color-settings/shop-color-settings';
import { ShopService } from '../../../services/shop-service';
import { Button } from '../../controls/button/button';

@Component({
  selector: 'app-shop-settings-component',
  imports: [ShopCardPreview, ShopColorSettings, Button],
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

  saveColors(): void {
    if (this.isSaving || !this.isColorsLoaded) return;
    this.isSaving = true;

    const shopId = this.shopService.currentShop?.id;
    if (!shopId) {
      this.isSaving = false;
      return;
    }

    this.shopService.saveColors(shopId, this.currentColors()).subscribe({
      next: () => {
        this.isSaving = false;
      },
      error: () => {
        this.isSaving = false;
      }
    });
  }

  cancelColors(): void {
    this.loadedColors.update(values => {
      const copy = [...values];
      return copy;
    });
  }

  defaultColors(): void {
    this.loadedColors.set([]);
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
