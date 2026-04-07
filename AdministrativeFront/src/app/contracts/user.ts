export interface User {
    id: string;
    username: string;
}

export interface ShopUser extends User {
    vkId: number;
}