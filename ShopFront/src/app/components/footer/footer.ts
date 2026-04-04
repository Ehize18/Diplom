import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ShopService } from '../../services/shop-service';

@Component({
  selector: 'app-footer',
  imports: [RouterLink],
  templateUrl: './footer.html',
  styleUrl: './footer.css',
})
export class Footer {
  shopId: string;
  constructor(private shopService: ShopService) {
    this.shopId = shopService.shopId();
  }
}
