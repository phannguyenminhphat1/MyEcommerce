import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateRoleDto {
  name?: string | null;
  description?: string | null;
}

export interface RoleDto extends EntityDto<string> {
  name?: string | null;
  description?: string | null;
}

export interface RoleInListDto extends EntityDto<string> {
  name?: string | null;
  description?: string | null;
}
