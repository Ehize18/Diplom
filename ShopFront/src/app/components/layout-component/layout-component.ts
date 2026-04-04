import { Component, computed, effect, input, signal, WritableSignal } from '@angular/core';
import { Header } from "../header/header";
import { Footer } from "../footer/footer";
import { RouterOutlet } from '@angular/router';
import { ShopService } from '../../services/shop-service';

@Component({
  selector: 'app-layout-component',
  imports: [Header, Footer, RouterOutlet],
  templateUrl: './layout-component.html',
  styleUrl: './layout-component.css',
})
export class LayoutComponent {
  readonly shopSignal: WritableSignal<string | null>;
  shopId = computed(() => this.shopSignal());

  constructor(private shopService: ShopService) {
    this.shopSignal = this.shopService.shopId;
    effect(() => console.log(this.shopId()));
  }
}
