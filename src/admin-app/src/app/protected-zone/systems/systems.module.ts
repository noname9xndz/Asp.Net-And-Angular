import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FunctionsComponent } from './functions/functions.component';
import { PermissionsComponent } from './permissions/permissions.component';
import { RolesComponent } from './roles/roles.component';
import { UsersComponent } from './users/users.component';



@NgModule({
  declarations: [
    FunctionsComponent,
    PermissionsComponent,
    RolesComponent,
    UsersComponent
  ],
  imports: [
    CommonModule
  ]
})
export class SystemsModule { }
