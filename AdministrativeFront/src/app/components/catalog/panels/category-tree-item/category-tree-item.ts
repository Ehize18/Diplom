import { Component, input, output } from '@angular/core';
import { Category } from '../../../../contracts/catalog';

export class TreeItem {
  id: string;
  title: string;
  childs: TreeItem[];
  parent: TreeItem | undefined;
  isExpanded = false;
  isActive = true;

  constructor(id: string, title: string, parent?: TreeItem, childs?: TreeItem[]) {
    this.id = id;
    this.title = title;
    this.parent = parent;
    if (childs) {
      this.childs = childs;
    }
    else {
      this.childs = [];
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
  itemSelected = output<TreeItem>();

  onItemClick(): void {
    if (this.item()) {
      this.itemSelected.emit(this.item()!);
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
