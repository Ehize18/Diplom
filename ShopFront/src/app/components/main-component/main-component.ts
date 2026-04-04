import { Component, effect, input, signal } from '@angular/core';
import { GoodComponent } from "../good-component/good-component";
import { CatalogService } from '../../services/catalog-service';
import { ShopService } from '../../services/shop-service';
import { Good } from '../../models/good';
import { CategorySliderComponent } from '../category-slider-component/category-slider-component';

@Component({
  selector: 'app-main-component',
  imports: [GoodComponent, CategorySliderComponent],
  templateUrl: './main-component.html',
  styleUrl: './main-component.css',
})
export class MainComponent {
  goods = signal<Good[]>([]);

  constructor(private catalogService: CatalogService, private shopService: ShopService) {
    effect(() => {
      if (this.shopService.shopId()) {
        this.catalogService.getGoods(this.shopService.shopId()!, '019d14ec-a990-7b2d-ae93-7935966479bc', false).subscribe(
          (value) => this.goods.set(value)
        );
      }
    });
  }
}
