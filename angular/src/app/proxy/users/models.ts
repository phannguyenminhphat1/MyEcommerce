import type { AuditedEntityDto } from '@abp/ng.core';

export interface CreateUserDto {
  name?: string | null;
  surname?: string | null;
  email?: string | null;
  userName?: string | null;
  password?: string | null;
  phoneNumber?: string | null;
}

export interface UpdateUserDto {
  name?: string | null;
  surname?: string | null;
  email?: string | null;
  phoneNumber?: string | null;
}

export interface UserDto extends AuditedEntityDto<string> {
  name?: string | null;
  userName?: string | null;
  email?: string | null;
  surname?: string | null;
  phoneNumber?: string | null;
  roles?: string[] | null;
  isActive?: boolean;
}

export interface UserInListDto extends AuditedEntityDto<string> {
  name?: string | null;
  surname?: string | null;
  email?: string | null;
  userName?: string | null;
  phoneNumber?: string | null;
}
