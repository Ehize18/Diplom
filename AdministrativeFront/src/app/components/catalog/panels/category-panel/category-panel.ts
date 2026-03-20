import { Component, effect, input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TreeItem } from '../category-tree-item/category-tree-item';
import { Button } from "../../../controls/button/button";

@Component({
  selector: 'app-category-panel',
  imports: [FormsModule, Button],
  templateUrl: './category-panel.html',
  styleUrl: './category-panel.css',
})
export class CategoryPanel {
  selectedCategory = input<TreeItem>();
  title: string | undefined;
  parentCategoryTitle: string | undefined;
  isActive: boolean | undefined;

  constructor() {
    effect(() => {
      this.title = this.selectedCategory()?.title;
      this.parentCategoryTitle = this.selectedCategory()?.parent?.title;
      this.isActive = this.selectedCategory()?.isActive;
    });
  }

  onSaveButtonClick(): void {
    const category = this.selectedCategory();
    if (category) {
      category.title = this.title!;
      category.isActive = this.isActive!;
    }
  }
}
