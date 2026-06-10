import type { BaseListFilterDto } from '../../models';
import type { AttributeType } from '../../my-ecommerce/product-attributes/attribute-type.enum';

export interface AddUpdateProductAttributeDto {
  productId?: string;
  attributeId?: string;
  dateTimeValue?: string | null;
  decimalValue?: number | null;
  intValue?: number | null;
  varcharValue?: string;
  textValue?: string;
}

export interface ProductAttributeListFilterDto extends BaseListFilterDto {
  productId?: string;
}

export interface ProductAttributeValueDto {
  id?: string;
  productId?: string;
  attributeId?: string;
  code?: string;
  dataType?: AttributeType;
  label?: string;
  dateTimeValue?: string | null;
  decimalValue?: number | null;
  intValue?: number | null;
  textValue?: string | null;
  varcharValue?: string | null;
  dateTimeId?: string | null;
  decimalId?: string | null;
  intId?: string | null;
  textId?: string | null;
  varcharId?: string | null;
}
