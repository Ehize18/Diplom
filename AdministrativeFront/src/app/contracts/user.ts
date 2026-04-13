export interface User {
    id: string;
    username: string;
}

export interface ShopUser extends User {
    vkId: number;
    ordersCount: number;
    totalSum: number;
    lastOrderDate: string | null;
}