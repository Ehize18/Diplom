import { Component, computed, input, OnInit } from '@angular/core';
import { Good } from '../../models/good';
import { ActivatedRoute, Router } from '@angular/router';
import { ShopService } from '../../services/shop-service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-good-component',
  imports: [],
  templateUrl: './good-component.html',
  styleUrl: './good-component.css',
})
export class GoodComponent implements OnInit {
  model = input<Good>();
  id: string = '';
  title: string = '';
  price: number = 0;
  oldPrice: number = 0;

  imageSrc = computed(() => {
    if (this.model()?.imageId) {
      return `${environment.IMAGES_URL}/${this.shopService.shopId()}/${this.model()?.imageId}`;
    }
    return 'placeholder.svg';
  });

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private shopService: ShopService
  ) {

  }

  ngOnInit(): void {
    const model = this.model();
    if (model) {
      this.id = model.id;
      this.title = model.title;
      this.price = model.price;
      this.oldPrice = model.oldPrice;
    }
  }



  onClick(): void {
    if (this.route.snapshot.url.find(x => x.path === 'catalog')) {
      this.router.navigate(['good', this.id], {relativeTo: this.route.parent});
    }
    else {
      this.router.navigate(['good', this.id], {relativeTo: this.route});
    }
  }
}
