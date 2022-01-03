import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MonthlyNewCommentsComponent } from './monthly-new-comments/monthly-new-comments.component';
import { MonthlyNewKbsComponent } from './monthly-new-kbs/monthly-new-kbs.component';
import { MonthlyNewMembersComponent } from './monthly-new-members/monthly-new-members.component';



@NgModule({
  declarations: [
    MonthlyNewCommentsComponent,
    MonthlyNewKbsComponent,
    MonthlyNewMembersComponent
  ],
  imports: [
    CommonModule
  ]
})
export class StatisticsModule { }
