export interface CreateShopRequest {
    title: string;
    description: string | undefined;
}

export interface Shop {
    id: string;
    title: string;
    description: string | undefined;
    vkGroupId: number | null;
    admins: ShopAdmin[];
}

export interface ShopAdmin {
    id: string;
    username: string;
    feature: AdminFeature;
}

export enum AdminFeature {
    CanAll = 15
}