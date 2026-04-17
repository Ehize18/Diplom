import { Component, effect, input, output, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Button } from "../../../controls/button/button";
import { ControlType, Popup, PopupConfig } from '../../../controls/popup/popup';
import { Category, Good } from '../../../../contracts/catalog';
import { LookupData } from '../../../controls/lookup/lookup';
import { ShopService } from '../../../../services/shop-service';
import { CatalogService } from '../../../../services/catalog-service';
import { Shop } from '../../../../contracts/shop';
import { TreeItem } from '../category-tree-item/category-tree-item';
import { environment } from '../../../../../environments/environment';

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
    this.orderCount = model.soldCount;
    this.iconUrl = 'placeholder.svg';
  }
}

@Component({
  selector: 'app-good-panel',
  imports: [Button, Popup, FormsModule],
  templateUrl: './good-panel.html',
  styleUrl: './good-panel.css',
})
export class GoodPanel {
  goodList = signal<GoodViewModel[]>([]);
  searchQuery = signal<string>('');
  filteredGoods = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) {
      return this.goodList();
    }
    return this.goodList().filter(g => g.title.toLowerCase().includes(query));
  });
  categories = input<Category[]>([]);
  selectedCategory = input<TreeItem | undefined>();
  goodsLoaded = output<GoodViewModel[]>();
  selectedGood = signal<GoodViewModel | null>(null);

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

  selectGood(good: GoodViewModel): void {
    if (this.selectedGood()?.id === good.id) {
      this.selectedGood.set(null);
    }
    else {
      this.selectedGood.set(good);
    }
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

    const currentShopId = this.shopService.currentShop!.id;

    if (tag === 'save') {
      const good: Good = {
        title: controls.title,
        description: controls.description,
        categoryId: controls.category.id,
        price: Number.parseFloat(controls.price),
        oldPrice: Number.parseFloat(controls.price),
        count: Number(controls.count),
        soldCount: 0,
        id: '',
        createdById: '',
        updatedById: '',
        imageId: '',
        createdAt: new Date(),
        updatedAt: new Date()
      };

      let image: File | undefined;

      if (controls.image && controls.image.file) {
        image = controls.image.file;
      }

      this.catalogService.createGood(
        currentShopId,
        good,
        image
      ).subscribe(
        (value) => {
          if (value) {
            this.getGoods(currentShopId, this.selectedCategory()?.id);
          }
        }
      );
    }
    else if (tag === 'edit') {
      const selectedGood = this.selectedGood();
      if (!selectedGood) {
        return;
      }

      selectedGood.model.title = controls.title;
      selectedGood.model.description = controls.description;
      selectedGood.model.categoryId = controls.category.id;
      selectedGood.model.price = Number.parseFloat(controls.price);
      selectedGood.model.oldPrice = Number.parseFloat(controls.price);
      selectedGood.model.count = Number(controls.count);

      let image: File | undefined;

      if (controls.image && controls.image.file) {
        image = controls.image.file;
      }

      this.catalogService.updateGood(
        currentShopId,
        selectedGood.model,
        image
      ).subscribe(
        (value) => {
          if (value) {
            this.getGoods(currentShopId, this.selectedCategory()?.id);
          }
        }
      );
    }
  }

  onDeleteGoodClick(): void {
    const selectedGood = this.selectedGood();
    if (!selectedGood) {
      return;
    }

    const currentShopId = this.shopService.currentShop!.id;
    this.catalogService.deleteGood(currentShopId, selectedGood.model).subscribe(
      () => {
        this.getGoods(currentShopId, this.selectedCategory()?.id);
        this.selectedGood.set(null);
      }
    );
  }

  getGoodImageSrc(good: GoodViewModel): string {
    if (good.model.imageId) {
      return `${environment.imageUrl}/${this.shopService.currentShop?.id}/${good.model.imageId}`
    }
    return 'placeholder.svg';
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

  getCategoryLookupData(categoryId: string | undefined): LookupData | undefined {
    if (!categoryId) {
      return undefined;
    }
    const category = this.categories().find(c => c.id === categoryId);
    if (category) {
      return {
        id: categoryId,
        caption: category.title
      }
    }
    return undefined;
  }

  private getEditPopupConfig(): PopupConfig {
    return {
      id: 'addGoodPopup',
      controls: [
        {
          type: ControlType.File,
          tag: 'image',
          caption: 'Изображение',
          value: this.selectedGood()?.model.imageId,
          lookupData: []
        },
        {
          type: ControlType.Input,
          tag: 'title',
          caption: 'Название',
          value: this.selectedGood()?.model.title,
          lookupData: []
        },
        {
          type: ControlType.Textarea,
          tag: 'description',
          caption: 'Описание',
          value: this.selectedGood()?.model.description,
          lookupData: []
        },
        {
          type: ControlType.Lookup,
          tag: 'category',
          caption: 'Категория',
          value: this.getCategoryLookupData(this.selectedGood()?.model.categoryId),
          lookupData: this.getLookupData()
        },
        {
          type: ControlType.Input,
          tag: 'price',
          caption: 'Цена',
          value: this.selectedGood()?.price,
          lookupData: []
        },
        {
          type: ControlType.Input,
          tag: 'count',
          caption: 'Количество на складе',
          value: this.selectedGood()?.count,
          lookupData: []
        }
      ],
      buttons: [
        {
          caption: 'Сохранить',
          tag: 'edit'
        },
        {
          caption: 'Закрыть',
          tag: 'close'
        }
      ],
      callback: this.onCloseModal.bind(this)
    }
  }

  getPopupConfig(): PopupConfig {
    if (this.selectedGood()) {
      return this.getEditPopupConfig();
    }
    return {
      id: 'addGoodPopup',
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
