export interface PageDto {
  id: string
  name: string
  label: string
  icon?: string
  route: string
  order: number
  parentId?: string
  userPermission: 0 | 1 | 2 | 3
}
