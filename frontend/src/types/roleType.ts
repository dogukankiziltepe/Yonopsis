export interface RoleTypeDto {
  id: string
  name: string
  isDefault: boolean
}

export interface PagePermissionDto {
  pageId: string
  pageName: string
  pageLabel: string
  pageRoute: string
  permissionLevel: 0 | 1 | 2 | 3
}

export interface RoleTypePermissionsDto {
  roleTypeId: string
  roleTypeName: string
  isDefault: boolean
  pages: PagePermissionDto[]
}

export const PermissionLevelLabel: Record<0 | 1 | 2 | 3, string> = {
  0: 'Yetkisiz',
  1: 'Sadece Görüntüle',
  2: 'Görüntüle & Oluştur',
  3: 'Tam Yetki',
}

export interface CreateRoleTypeDto { name: string }
export interface UpdateRoleTypeDto { name: string }
export interface SetRolePermissionDto {
  pageId: string
  permissionLevel: 0 | 1 | 2 | 3
}
