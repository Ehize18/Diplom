import { Component, effect, input, OnInit, output, signal } from '@angular/core';
import { CategoryTreeItem, TreeItem } from '../category-tree-item/category-tree-item';
import { ShopService } from '../../../../services/shop-service';
import { CatalogService } from '../../../../services/catalog-service';
import { Category } from '../../../../contracts/catalog';
import { Shop } from '../../../../contracts/shop';
import { toObservable } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-category-tree-panel',
  imports: [CategoryTreeItem],
  templateUrl: './category-tree-panel.html',
  styleUrl: './category-tree-panel.css',
})
export class CategoryTreePanel {
  tree = signal<TreeItem[]>([]);
  categorySaved = input<Category>();
  reloadTrigger = input(0);
  selectedItem: TreeItem | undefined;
  selectedItemId: string | undefined;
  itemSelected = output<TreeItem | undefined>();
  loadedItems: Category[] = [];
  itemsLoaded = output<Category[]>();

  constructor(
    private shopService: ShopService,
    private catalogService: CatalogService
  ) {
    let currentShopV: Shop | undefined;
    let isAlreadyLoaded = false;
    this.shopService.getCurrentShopObservable().subscribe((currentShop) => {
      currentShopV = currentShop;
      if (currentShopV) {
        isAlreadyLoaded = true;
        this.catalogService.getCategories(currentShopV.id).subscribe((value) => {
          if (value && value.isSuccess) {
            this.loadCategories(value.results);
          }
        });
      }
    });
    toObservable(this.categorySaved).subscribe(
      () => {
        console.log(this.categorySaved());
        if (currentShopV && !isAlreadyLoaded) {
          this.catalogService.getCategories(currentShopV.id).subscribe((value) => {
            if (value && value.isSuccess) {
              this.loadCategories(value.results);
            }
          });
        }
        isAlreadyLoaded = false;
      }
    );
    effect(() => {
      const trigger = this.reloadTrigger();
      if (currentShopV && trigger > 0) {
        this.catalogService.getCategories(currentShopV.id).subscribe((value) => {
          if (value && value.isSuccess) {
            this.loadCategories(value.results);
          }
        });
      }
    });
  }

  private loadCategories(categories: Category[]): void {
    const treeItems: TreeItem[] = [];
    this.loadedItems = [];
    this.selectedItemId = this.selectedItem?.id;
    this.onItemSelected(undefined);
    categories.forEach((category) => {
      const treeItem = new TreeItem(category)
      treeItems.push(treeItem);
      this.loadChildCategories(treeItem);
      this.loadedItems.push(category);
      if (treeItem.id === this.selectedItemId) {
        this.onItemSelected(treeItem);
      }
    });
    this.tree.set(treeItems);
  }

  private loadChildCategories(treeItem: TreeItem) {
    this.catalogService.getChildCategories(
        this.shopService.currentShop!.id, treeItem.category.id
      ).subscribe((response) => {
        if (response && response.isSuccess) {
          treeItem.childs.set([]);
          response.results.forEach((category) => {
            const treeItemChild = new TreeItem(category);
            this.loadedItems.push(category);
            treeItemChild.parent = treeItem;
            treeItem.childs.update((childs) => {
              childs.push(treeItemChild);
              return childs;
            });
            if (treeItemChild.id === this.selectedItemId) {
              this.onItemSelected(treeItemChild);
              treeItem.expand();
            }
            this.loadChildCategories(treeItemChild);
          });
        }
        this.itemsLoaded.emit(this.loadedItems);
      });
  }

  onItemSelected(item: TreeItem | undefined): void {
    this.selectedItem = item;
    this.itemSelected.emit(item);
  }
}
