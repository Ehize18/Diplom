import { Component, computed, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CatalogService } from '../../services/catalog-service';
import { Good } from '../../models/good';
import { ShopService } from '../../services/shop-service';
import { BasketService } from '../../services/basket-service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-good-card',
  imports: [],
  templateUrl: './good-card.html',
  styleUrl: './good-card.css',
})
export class GoodCard implements OnInit {
  id: string = '';
  model = signal<Good | null>(null);
  shopId = '';
  imageSrc = computed(() => {
    if (this.model()?.imageId) {
      return `${environment.IMAGES_URL}/${this.shopId}/${this.model()?.imageId}`;
    }
    return 'placeholder.svg';
  });

  inBasket = computed(() => {
    const basket = this.basketService.currentBasket();
    return basket.items.find(x => x.goodId === this.model()?.id);
  });

  oldPrice = computed(() => {
    const model = this.model();
    if (model && model.oldPrice && model.oldPrice > 0 && model.oldPrice !== model.price) {
      return model.oldPrice;
    }
    return null
  });

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private catalogService: CatalogService,
    private shopService: ShopService,
    private basketService: BasketService
  ) {
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('goodId')!;
    this.shopId = this.route.parent!.snapshot.paramMap.get('shopUuid')!;

    this.route.paramMap.subscribe(
        params => {
          this.id = params.get('goodId')!;
          this.catalogService.getGoodById(this.shopId, this.id)
            .subscribe(
              (good) => {
                console.log(good)
                this.model.set(good);
              }
            );
        }
    );
  }

  onAddToBasketClick() {
    if (this.inBasket()) {
      this.router.navigate(['basket'], { relativeTo: this.route.parent })
    }
    else {
      const shopId = this.shopService.shopId();
      this.basketService.addGoodToBasket(
        shopId,
        this.model()!.id,
        1
      ).subscribe(() => {
        this.basketService.loadBasket(shopId);
      });
    }
  }
}
