import { Component, signal } from '@angular/core';
import { Button } from "../../controls/button/button";
import { CategoryTreePanel } from '../panels/category-tree-panel/category-tree-panel';
import { CategoryPanel } from '../panels/category-panel/category-panel';
import { TreeItem } from '../panels/category-tree-item/category-tree-item';
import { CategoryStaticsticType, CategoryStatisticPanel } from "../panels/category-statistic-panel/category-statistic-panel";
import { GoodPanel, GoodViewModel } from "../panels/good-panel/good-panel";
import { ControlType, Popup, PopupConfig } from "../../controls/popup/popup";
import { Category } from '../../../contracts/catalog';
import { LookupData } from '../../controls/lookup/lookup';
import { ShopService } from '../../../services/shop-service';
import { CatalogService } from '../../../services/catalog-service';
import { GoodPropertiesComponent } from '../panels/good-properties-component/good-properties-component';

@Component({
  selector: 'app-catalog-component',
  imports: [Button, CategoryTreePanel, CategoryPanel, CategoryStatisticPanel, GoodPanel, Popup, GoodPropertiesComponent],
  templateUrl: './catalog-component.html',
  styleUrl: './catalog-component.css',
})
export class CatalogComponent {
  selectedCategory: TreeItem | undefined;
  statisticType = CategoryStaticsticType;
  isModalOpened = signal(false);
  loadedCategories: Category[] = [];

  constructor(private shopService: ShopService, private catalogService:CatalogService) {
  }

  onSelectedCategoryChange(category: TreeItem | undefined): void {
    this.selectedCategory = category;
  }

  onCategoriesLoaded(categories: Category[]) {
    this.loadedCategories = categories;
  }

  shopModal(): void {
    this.isModalOpened.set(true);
  }

  goodsTotal = 0;
  goodsInCategory = 0;
  ordersTotal = 0;
  ordersInCategory = 0;

  onGoodsLoaded(goods: GoodViewModel[]): void {
    console.log(goods);
    let goodsInCategory = 0;
    let ordersTotal = 0;
    let ordersInCategory = 0;
    goods.forEach(g => {
      ordersTotal += g.orderCount;
      if (g.model.categoryId === this.selectedCategory?.category.id) {
        goodsInCategory += 1;
        ordersInCategory += g.orderCount;
      }
    });
    this.goodsTotal = goods.length;
    this.goodsInCategory = goodsInCategory;
    this.ordersTotal = ordersTotal;
    this.ordersInCategory = ordersInCategory;
  }

  onCloseModal(tag: string, controls: any): void {
    console.log(tag);
    console.log(controls);
    this.isModalOpened.set(false);
    if (tag === 'save') {
      const category: Category = {
        title: controls.title,
        description: controls.description,
        parentCategoryId: controls.parentCategory?.id,
        isActive: controls.isActive,
        id: '',
        createdAt: new Date(),
        updatedAt: new Date(),
        createdById: '',
        updatedById: '',
        imageId: ''
      }

      let image: File | undefined;

      if (controls.image && controls.image.file) {
        image = controls.image.file;
      }

      this.catalogService.createCategory(
        this.shopService.currentShop!.id,
        category,
        image
      ).subscribe(
        (value) => {
          if (value) {
            console.log(value);
            this.categorySaved(category);
          }
        }
      )
    }
  }

  savedCategory = signal<Category | undefined>(undefined);

  categorySaved(category: Category): void {
    this.savedCategory.set(category);
  }

  getLookupData(): LookupData[] {
    const data: LookupData[] = [];
    this.loadedCategories.forEach(
      category => {
        data.push({
          id: category.id,
          caption: category.title
        });
      }
    );
    return data;
  }

  getPopupConfig(): PopupConfig {
    return {
      id: 'category-create',
      controls: [
        {
          type: ControlType.File,
          tag: 'image',
          caption: 'Изображение',
          value: '',
          lookupData: []
        },
        {
          type: ControlType.Input,
          tag: 'title',
          caption: 'Название категории',
          value: '',
          lookupData: []
        },
        {
          type: ControlType.Lookup,
          tag: 'parentCategory',
          caption: 'Родительская категория',
          value: undefined,
          lookupData: this.getLookupData()
        },
        {
          type: ControlType.Boolean,
          tag: 'isActive',
          caption: 'Активно',
          value: true,
          lookupData: []
        }
      ],
      buttons: [
        {
          caption: 'Сохранить',
          tag: 'save'
        },
        {
          caption: 'Закрыть',
          tag: 'close'
        }
      ],
      callback: this.onCloseModal.bind(this)
    };
  }
}
