import { Component, OnInit, output, signal } from '@angular/core';
import { CategoryTreeItem, TreeItem } from '../category-tree-item/category-tree-item';

@Component({
  selector: 'app-category-tree-panel',
  imports: [CategoryTreeItem],
  templateUrl: './category-tree-panel.html',
  styleUrl: './category-tree-panel.css',
})
export class CategoryTreePanel implements OnInit {
  tree = signal<TreeItem[]>([]);
  selectedItem: TreeItem | undefined;
  itemSelected = output<TreeItem>();

  ngOnInit(): void {
    this.tree.update((tree) => {

      const f1 = new TreeItem('1', 'Шуруповёрты')
      const f2 = new TreeItem('2', 'Телефоны');
      const f3 = new TreeItem('3', 'Андроиды', f2)

      f2.childs.push(f3);

      tree.push(f1, f2);

      return tree;
    });
  }

  onItemSelected(item: TreeItem): void {
    this.selectedItem = item;
    this.itemSelected.emit(item);
  }
}
