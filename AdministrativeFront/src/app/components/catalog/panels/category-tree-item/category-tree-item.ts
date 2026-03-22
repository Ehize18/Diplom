import { Component, input, output, signal, WritableSignal } from '@angular/core';
import { Category } from '../../../../contracts/catalog';

export class TreeItem {
  id: string;
  title: string;
  description: string | null;
  childs: WritableSignal<TreeItem[]>;
  parent: TreeItem | undefined;
  parentId: string | null;
  isExpanded = false;
  isActive = true;
  category: Category

  constructor(category: Category) {
    this.id = category.id;
    this.title = category.title;
    this.description = category.description;
    this.isActive = category.isActive;
    this.parentId = category.parentCategoryId;
    this.childs = signal<TreeItem[]>([]);
    this.category = category;
  }

  expand(): void {
    this.isExpanded = true;
    if (this.parent) {
      this.parent.expand();
    }
  }
}

@Component({
  selector: 'app-category-tree-item',
  imports: [],
  templateUrl: './category-tree-item.html',
  styleUrl: './category-tree-item.css',
})
export class CategoryTreeItem {
  item = input<TreeItem>();
  selectedItem = input<TreeItem>();
  itemSelected = output<TreeItem | undefined>();

  onItemClick(): void {
    if (this.item()) {
      if (this.item()?.id === this.selectedItem()?.id) {
        this.itemSelected.emit(undefined);
      }
      else {
        this.itemSelected.emit(this.item()!);
      }
    }
  }

  onExpandClick(): void {
    const item = this.item()!;
    item.isExpanded = !item.isExpanded
  }

  isSelected(): boolean {
    return this.selectedItem() === this.item();
  }
}
