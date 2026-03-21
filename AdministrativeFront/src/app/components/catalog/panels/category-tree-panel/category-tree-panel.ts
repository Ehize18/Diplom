import { Component, OnInit, output, signal } from '@angular/core';
import { CategoryTreeItem, TreeItem } from '../category-tree-item/category-tree-item';
import { ShopService } from '../../../../services/shop-service';
import { CatalogService } from '../../../../services/catalog-service';
import { Category } from '../../../../contracts/catalog';

@Component({
  selector: 'app-category-tree-panel',
  imports: [CategoryTreeItem],
  templateUrl: './category-tree-panel.html',
  styleUrl: './category-tree-panel.css',
})
export class CategoryTreePanel {
  tree = signal<TreeItem[]>([]);
  selectedItem: TreeItem | undefined;
  itemSelected = output<TreeItem | undefined>();

  constructor(
    private shopService: ShopService,
    private catalogService: CatalogService
  ) {
    shopService.getCurrentShopObservable().subscribe((currentShop) => {
      if (currentShop) {
        catalogService.getCategories(currentShop.id).subscribe((value) => {
          if (value && value.isSuccess) {
            this.loadCategories(value.results);
          }
        });
      }
    });
  }

  private loadCategories(categories: Category[]): void {
    const treeItems: TreeItem[] = [];
    categories.forEach((category) => {
      treeItems.push(new TreeItem(category.id, category.title));
    })
    this.tree.set(treeItems);
    this.onItemSelected(undefined);
  }

  onItemSelected(item: TreeItem | undefined): void {
    this.selectedItem = item;
    this.itemSelected.emit(item);
  }
}
