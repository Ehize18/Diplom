import { Component } from '@angular/core';
import { Button } from "../../controls/button/button";
import { CategoryTreePanel } from '../panels/category-tree-panel/category-tree-panel';
import { CategoryPanel } from '../panels/category-panel/category-panel';
import { TreeItem } from '../panels/category-tree-item/category-tree-item';
import { CategoryStaticsticType, CategoryStatisticPanel } from "../panels/category-statistic-panel/category-statistic-panel";
import { GoodPanel } from "../panels/good-panel/good-panel";

@Component({
  selector: 'app-catalog-component',
  imports: [Button, CategoryTreePanel, CategoryPanel, CategoryStatisticPanel, GoodPanel],
  templateUrl: './catalog-component.html',
  styleUrl: './catalog-component.css',
})
export class CatalogComponent {
  selectedCategory: TreeItem | undefined;
  statisticType = CategoryStaticsticType;

  onSelectedCategoryChange(category: TreeItem): void {
    this.selectedCategory = category;
  }
}
