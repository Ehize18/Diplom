import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CatalogService } from '../../services/catalog-service';
import { ShopService } from '../../services/shop-service';
import { GoodComponent } from '../good-component/good-component';
import { Good, GoodCategory } from '../../models/good';
import { FormsModule } from '@angular/forms';

interface FilterValue {
  id: string;
  title: string;
}

@Component({
  selector: 'app-catalog-component',
  imports: [GoodComponent, FormsModule],
  templateUrl: './catalog-component.html',
  styleUrl: './catalog-component.css',
})
export class CatalogComponent implements OnInit {
  goods = signal<Good[]>([]);
  isFiltersPopupVisible = signal<boolean>(false);
  category = signal<GoodCategory | null>(null);
  showedFilterValues = signal<FilterValue[]>([]);
  selectedFilterValues = signal<string[]>([]);

  constructor(
    private route: ActivatedRoute,
    private catalogService: CatalogService,
    private shopService: ShopService
  ) {

  }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(
      params => {
        const categoryId = params.get('categoryId');
        if (categoryId) {
          this.catalogService.getGoods(
            this.shopService.shopId(),
            categoryId,
            false
          ).subscribe(
            goods => {
              console.log(goods);
              this.goods.set(goods);
            }
          );
          this.catalogService.getCategoryById(
            this.shopService.shopId(),
            categoryId,
            false
          ).subscribe(
            category => {
              this.category.set(category);
            }
          );
        }
      }
    );
  }

  closeFilter(result: boolean): void {
    this.isFiltersPopupVisible.set(false);
    if (result) {
      console.log(result);
    }
  }

  onFiltersClick(): void {
    this.isFiltersPopupVisible.set(true);
  }

  onFilterSubmit(): void {
    console.log(this.selectedFilterValues());
  }
}
