import { Routes } from '@angular/router';
import { HomeComponent } from './components/home-component/home-component';
import { LayoutComponent } from './components/layout-component/layout-component';
import { shopExistsGuard } from './guards/shop-exists-guard';
import { MainComponent } from './components/main-component/main-component';
import { GoodCard } from './components/good-card/good-card';
import { CategoryComponent } from './components/category-component/category-component';
import { BasketComponent } from './components/basket-component/basket-component';
import { VkComponent } from './components/vk-component/vk-component';
import { ProfileComponent } from './components/profile-component/profile-component';
import { CatalogComponent } from './components/catalog-component/catalog-component';
import { OrderComponent } from './components/order-component/order-component';
import { OrdersComponent } from './components/orders-component/orders-component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent
    },
    {
        path: 'vk',
        component: VkComponent
    },
    {
        path: ':shopUuid', component: LayoutComponent,
        canActivate: [shopExistsGuard],
        children: [
            {
                path: '',
                component: MainComponent
            },
            {
                path: 'good/:goodId',
                component: GoodCard
            },
            {
                path: 'category/:categoryId',
                component: CategoryComponent
            },
            {
                path: 'category',
                component: CategoryComponent
            },
            {
                path: 'basket',
                component: BasketComponent
            },
            {
                path: 'profile',
                component: ProfileComponent
            },
            {
                path: 'catalog',
                component: CatalogComponent
            },
            {
                path: 'orders',
                component: OrdersComponent
            },
            {
                path: 'order/:orderId',
                component: OrderComponent
            },
            {
                path: 'order',
                component: OrderComponent
            }
        ]
    }
];
