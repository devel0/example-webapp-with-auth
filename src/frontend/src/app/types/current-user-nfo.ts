import { UserPermission } from "../../api";

export interface CurrentUserNfo {
  userName: string
  email: string
  roles: string[]
  permissions: UserPermission[]
  refreshTokenExpiration: string
}