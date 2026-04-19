import { Routes } from '@angular/router';
import { HomeComponent } from './components/home-component/home-component';
import { ShopSettingsComponent } from './components/shop-settings/shop-settings-component/shop-settings-component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { 
        path: 'shop', 
        loadComponent:() => import('./components/shop/shop-layout-component/shop-layout-component').then(v => v.ShopLayoutComponent),
        children: [
            {
                path: '',
                loadComponent:() => import('./components/shop/shop-component/shop-component').then(v => v.ShopComponent)
            },
            {
                path: 'catalog',
                loadComponent:() => import('./components/catalog/catalog-component/catalog-component').then(v => v.CatalogComponent)
            },
            {
                path: 'orders',
                loadComponent:() => import('./components/orders/orders-component/orders-component').then(v => v.OrdersComponent)
            },
            {
                path: 'clients',
                loadComponent:() => import('./components/clients/clients-component').then(v => v.ClientsComponent)
            },
            {
                path: 'settings',
                loadComponent:() => import('./components/shop-settings/shop-settings-component/shop-settings-component').then(v => v.ShopSettingsComponent)
            }
        ]
    },
    { path: '**', redirectTo: '' }
];
