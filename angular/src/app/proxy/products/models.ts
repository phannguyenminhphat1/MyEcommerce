import { BaseListFilterDto } from '@proxy/models';
import type { ProductType } from '../my-ecommerce/products/product-type.enum';
import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateProductDto {
  manufacturerId?: string;
  name?: string;
  code?: string;
  slug?: string;
  productType?: ProductType;
  sku?: string;
  sortOrder?: number;
  visiblity?: boolean;
  isActive?: boolean;
  categoryId?: string;
  seoMetaDescription?: string;
  description?: string;
  thumbnailPicture?: string;
}

export interface ProductDto {
  name?: string;
  code?: string;
  slug?: string;
  sortOrder?: number;
  coverPicture?: string;
  visibility?: boolean;
  isActive?: boolean;
  parentId?: string | null;
  seoMetaDescription?: string;
  id?: string;
}

export interface ProductInListDto extends EntityDto<string> {
  manufacturerId?: string;
  name?: string;
  code?: string;
  slug?: string;
  productType?: ProductType;
  sku?: string;
  sortOrder?: number;
  visiblity?: boolean;
  isActive?: boolean;
  categoryId?: string;
  thumbnailPicture?: string;
}

export interface ProductListFilterDto extends BaseListFilterDto {
  categoryId?: string;
}
