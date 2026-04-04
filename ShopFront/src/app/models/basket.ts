export interface AddGoodToBasketRequest {
    goodId: string,
    count: number | null | undefined
}

export interface Basket {
    items: BasketItem[];
}

export interface BasketItem {
    basketItemId: string;
    goodId: string;
    title: string;
    price: number;
    count: number;
    imageId: string | null;
}