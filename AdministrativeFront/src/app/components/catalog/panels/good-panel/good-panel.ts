import { Component, effect, input, OnInit, output, signal } from '@angular/core';
import { Button } from "../../../controls/button/button";
import { ControlType, Popup, PopupConfig } from '../../../controls/popup/popup';
import { Category, Good } from '../../../../contracts/catalog';
import { LookupData } from '../../../controls/lookup/lookup';
import { ShopService } from '../../../../services/shop-service';
import { CatalogService } from '../../../../services/catalog-service';
import { Shop } from '../../../../contracts/shop';
import { TreeItem } from '../category-tree-item/category-tree-item';

export class GoodViewModel {
  id: string;
  iconUrl: string;
  title: string;
  category: string | undefined;
  price: number;
  count: number;
  orderCount: number;
  model: Good

  constructor(model: Good, category?: string) {
    this.model = model;
    this.id = model.id;
    this.title = model.title;
    this.category = category;
    this.price = model.price;
    this.count = model.count;
    this.orderCount = 0;
    this.iconUrl = 'placeholder.svg';
  }
}

@Component({
  selector: 'app-good-panel',
  imports: [Button, Popup],
  templateUrl: './good-panel.html',
  styleUrl: './good-panel.css',
})
export class GoodPanel {
  goodList = signal<GoodViewModel[]>([]);
  categories = input<Category[]>([]);
  selectedCategory = input<TreeItem | undefined>();
  goodsLoaded = output<GoodViewModel[]>();

  constructor(
    private shopService: ShopService,
    private catalogService: CatalogService
  ) {
    let currentShop: Shop | undefined;
    this.shopService.getCurrentShopObservable().subscribe(
      (shop) => { 
        currentShop = shop
        this.goodList.set([]);
      }
    );
    effect(() => {
      if (currentShop && this.categories().length > 0) {
        this.getGoods(currentShop?.id, this.selectedCategory()?.id)
      }
    });
  }

  getGoods(shopId: string, categoryId?: string): void {
    this.catalogService.getGoods(shopId, 'Title', true, categoryId)
      .subscribe(
        (response) => {
          this.loadGoods(response.results);
          const selectedCategory = this.selectedCategory();
          if (selectedCategory) {
            this.getChildCategoriesGoods(selectedCategory);
          }
        }
      );
  }

  getChildCategoriesGoods(category: TreeItem): void {
    if (category) {
      category.childs().forEach(child => {
        this.catalogService.getGoods(
          this.shopService.currentShop!.id, 'Title', true, child.id
        ).subscribe(
          (response) => {
            this.loadMoreGoods(response.results);
            this.getChildCategoriesGoods(child);
          }
        );
      });
    }
  }

  loadGoods(goods: Good[]): void {
    this.goodList.update(
      () => {
        const newGoods: GoodViewModel[] = [];
        goods.forEach(
          (good) => {
            const category = this.categories().find(c => c.id === good.categoryId);
            newGoods.push(new GoodViewModel(good, category?.title));
          }
        );
        this.goodsLoaded.emit(newGoods);
        return newGoods;
      }
    )
  }

  loadMoreGoods(goods: Good[]): void {
    this.goodList.update(
      (goodList) => {
        goods.forEach(
          (good) => {
            const category = this.categories().find(c => c.id === good.categoryId);
            goodList.push(new GoodViewModel(good, category?.title));
          }
        );
        this.goodsLoaded.emit(goodList);
        return goodList;
      }
    )
  }

  onCloseModal(tag: string, controls: any): void {
    console.log(tag);
    console.log(controls);
    this.isModalOpened.set(false);
    if (tag === 'save') {
      const good: Good = {
        title: controls.title,
        description: controls.description,
        categoryId: controls.category.id,
        price: Number.parseFloat(controls.price),
        oldPrice: Number.parseFloat(controls.price),
        count: Number(controls.count),
        id: '',
        createdById: '',
        updatedById: '',
        createdAt: new Date(),
        updatedAt: new Date()
      };

      const currentShopId = this.shopService.currentShop!.id;

      this.catalogService.createGood(
        currentShopId,
        good
      ).subscribe(
        (value) => {
          if (value && this.selectedCategory()?.id === good.categoryId) {
            this.getGoods(currentShopId, this.selectedCategory()?.id);
          }
        }
      );
    }
  }

  showModal(): void {
    this.isModalOpened.set(true);
  }

  isModalOpened = signal(false);

  getLookupData(): LookupData[] {
    const result: LookupData[] = [];
    this.categories().forEach(
      (category) => {
        result.push({
          id: category.id,
          caption: category.title
        });
      }
    );
    return result;
  }

  getSelectedCategoryLookupData(): LookupData | undefined {
    const selectedCategory = this.selectedCategory();
    if (selectedCategory) {
      return {
        id: selectedCategory.id,
        caption: selectedCategory.title
      }
    }
    return undefined;
  }

  getPopupConfig(): PopupConfig {
    return {
      id: 'addGoodPopup',
      controls: [
        {
          type: ControlType.Input,
          tag: 'title',
          caption: 'Название',
          value: '',
          lookupData: []
        },
        {
          type: ControlType.Textarea,
          tag: 'description',
          caption: 'Описание',
          value: '',
          lookupData: []
        },
        {
          type: ControlType.Lookup,
          tag: 'category',
          caption: 'Категория',
          value: this.getSelectedCategoryLookupData(),
          lookupData: this.getLookupData()
        },
        {
          type: ControlType.Input,
          tag: 'price',
          caption: 'Цена',
          value: 0,
          lookupData: []
        },
        {
          type: ControlType.Input,
          tag: 'count',
          caption: 'Количество на складе',
          value: 0,
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
    }
  }
}
