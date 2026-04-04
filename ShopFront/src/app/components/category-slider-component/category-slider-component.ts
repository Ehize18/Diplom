import { Component, OnInit, signal } from '@angular/core';
import { CatalogService } from '../../services/catalog-service';
import { ShopService } from '../../services/shop-service';
import { GoodCategory } from '../../models/good';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-category-slider-component',
  imports: [],
  templateUrl: './category-slider-component.html',
  styleUrl: './category-slider-component.css',
})
export class CategorySliderComponent implements OnInit {
  mainCategories = signal<GoodCategory[]>([]);

  constructor(
    private catalogService: CatalogService,
    private shopService: ShopService,
    private router: Router,
    private route: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.catalogService.getCategoriesByParent(this.shopService.shopId(), null)
      .subscribe((categories) => this.mainCategories.set(categories));
  }

  onCategoryClick(categoryId: string): void {
    this.router.navigate(['category', categoryId], {relativeTo: this.route});
  }

  getCategorySrc(category: GoodCategory): string {
    if (category.imageId) {
      return `${environment.IMAGES_URL}/${this.shopService.shopId()}/${category.imageId}`;
    }
    return 'placeholder.svg';
  }
}
