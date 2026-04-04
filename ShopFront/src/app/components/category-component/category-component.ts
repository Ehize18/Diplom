import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GoodCategory } from '../../models/good';
import { CatalogService } from '../../services/catalog-service';

@Component({
  selector: 'app-category-component',
  imports: [],
  templateUrl: './category-component.html',
  styleUrl: './category-component.css',
})
export class CategoryComponent implements OnInit {
  model = signal<GoodCategory | null>(null);
  id: string | null = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private catalogService: CatalogService
  ) {
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.id = params.get('categoryId');
      const shopId = this.route.parent!.snapshot.paramMap.get('shopUuid')!;
      if (this.id) {
        this.catalogService.getCategoryById(shopId, this.id, true)
          .subscribe(
            (category) => this.model.set(category)
          );
      }
      else {
        this.catalogService.getCategoriesByParent(shopId, null)
          .subscribe(
              (categories) => this.model.set({
                id: '',
                parentCategoryId: '',
                title: 'Каталог',
                description: '',
                childs: categories,
                imageId: null
              })
            );
      }
    });
  }

  onCategoryClick(categoryId: string): void {
    if (this.id) {
      this.router.navigate(['..', categoryId], { relativeTo: this.route });
    }
    else {
      this.router.navigate([categoryId], { relativeTo: this.route });
    }
  }

  onShowCatalogClick(categoryId: string): void {
    this.router.navigate(['catalog'], {
      queryParams: { categoryId: categoryId },
      relativeTo: this.route.parent
    });
  }
}
