import { Component, OnInit, signal } from '@angular/core';
import { BasketService } from '../../services/basket-service';
import { BasketItem } from '../../models/basket';
import { toObservable } from '@angular/core/rxjs-interop';
import { FormsModule } from "@angular/forms";
import { ShopService } from '../../services/shop-service';
import { environment } from '../../../environments/environment';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-basket-component',
  imports: [FormsModule],
  templateUrl: './basket-component.html',
  styleUrl: './basket-component.css',
})
export class BasketComponent {
  basketItems = signal<BasketItem[]>([]);

  constructor(
    private shopService: ShopService,
    private basketService: BasketService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    toObservable(this.basketService.currentBasket).subscribe(
      basket => {
        this.basketItems.set(basket.items);
      }
    )
  }

  getImageSrc(basketItem: BasketItem): string {
    if (basketItem.imageId) {
      return `${environment.IMAGES_URL}/${this.shopService.shopId()}/${basketItem.imageId}`;
    }
    return 'placeholder.svg';
  }

  onBasketItemCountChange(basketItemId: string): void {
    const item = this.basketItems().find(x => x.basketItemId === basketItemId);
    if (item) {
      if (item.count <= 0) {
        item.count = 1;
      }
      this.basketService.changeBasketItemCount(
        this.shopService.shopId(),
        item
      ).subscribe(
        count => {
          if (count < 0) {
            console.error('Count error');
          }
        }
      );
    }
  }

  onDeleteBasketItemClick(basketItemId: string): void {
    const item = this.basketItems().find(x => x.basketItemId === basketItemId);
    if (item) {
      this.basketService.deleteBasketItem(
        this.shopService.shopId(),
        item
      );
    }
  }

  onConfirmOrderButtonClick(): void {
    this.router.navigate(['order'],  {
      relativeTo: this.route.parent,
      queryParams: {
        isNew: true
      }
    });
  }
}
