import { Component, effect, HostListener, input, OnInit, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TreeItem } from '../category-tree-item/category-tree-item';
import { Button } from "../../../controls/button/button";
import { CatalogService } from '../../../../services/catalog-service';
import { ShopService } from '../../../../services/shop-service';
import { Category } from '../../../../contracts/catalog';
import { Lookup, LookupData } from "../../../controls/lookup/lookup";
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-category-panel',
  imports: [FormsModule, Button, Lookup],
  templateUrl: './category-panel.html',
  styleUrl: './category-panel.css',
})
export class CategoryPanel {
  selectedCategory = input<TreeItem>();
  loadedCategoriesInput = input<Category[]>();
  loadedCategories = signal<Category[]>([]);
  categorySaved = output<Category>();
  title: string | undefined;
  description: string | undefined | null;
  parentCategory: Category | undefined;
  parentCategoryTitle: string | undefined;
  parentCategoryId: string | undefined;
  isActive: boolean | undefined;
  imageSrc = signal<string>('placeholder.svg');
  newImage: File | undefined;

  constructor(
    private shopService: ShopService,
    private catalogService: CatalogService
  ) {
    effect(() => {
      this.title = this.selectedCategory()?.title;
      this.description = this.selectedCategory()?.description;
      this.parentCategory = this.selectedCategory()?.parent?.category;
      this.parentCategoryTitle = this.parentCategory?.title;
      this.parentCategoryId = this.parentCategory?.id;
      this.isActive = this.selectedCategory()?.isActive;
      if (this.selectedCategory()?.category.imageId) {
        this.setImageSrc(this.selectedCategory()!.category.imageId!);
      } else {
        this.imageSrc.set('placeholder.svg');
      }
      this.newImage = undefined;
      const loadedCat = this.loadedCategoriesInput();
      if (loadedCat) {
        this.loadedCategories.set(loadedCat);
      }
    });
  }

  setImageSrc(imageId: string) {
    this.imageSrc.set(`${environment.imageUrl}/${this.shopService.currentShop?.id}/${imageId}`);
  }

  onImageChanged(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.newImage = file;
      const reader = new FileReader();
      reader.onload = (e: any) => this.imageSrc.set(e.target.result);
      reader.readAsDataURL(file);
    }
  }

  getLookupData(): LookupData[] {
    const data: LookupData[] = [];
    const categories = this.loadedCategories();
    categories.forEach(
      (category) => data.push({ id: category.id, caption: category.title })
    );
    return data;
  }

  getDefaultLookupData(): LookupData | undefined {
    if (this.parentCategory) {
      return {
        id: this.parentCategory.id,
        caption: this.parentCategory.title
      }
    }
    return undefined;
  }

  parentCategoryChanged(data: LookupData | undefined) {
    if (data) {
      const category = this.loadedCategories().find(
        (category) => category.id === data.id
      );
      this.parentCategory = category;
      this.parentCategoryTitle = category?.title;
    }
    else {
      this.parentCategory = undefined;
      this.parentCategoryTitle = undefined;
    }
  }

  onSaveButtonClick(): void {
    const category = this.selectedCategory();
    if (category && category.category) {
      category.category.title = this.title!;
      category.category.description = this.description || null;
      category.category.isActive = this.isActive!;
      category.category.parentCategoryId = this.parentCategory?.id || null;
      this.catalogService.updateCategory(this.shopService.currentShop?.id!, category.category, this.newImage).subscribe(
        (value) => {
          if (value) {
            this.categorySaved.emit(this.selectedCategory()?.category!);
          }
        }
      );
    }
  }

  onDeleteButtonClick(): void {
    const category = this.selectedCategory();
    if (category && category.category) {
      this.catalogService.deleteCategory(this.shopService.currentShop?.id!, category.category).subscribe(
        () => {
          this.categorySaved.emit(this.selectedCategory()?.category!);
        }
      );
    }
  }
}
