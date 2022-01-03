import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoriesComponent } from './categories/categories.component';
import { KnowledgeBasesComponent } from './knowledge-bases/knowledge-bases.component';



@NgModule({
  declarations: [
    CategoriesComponent,
    KnowledgeBasesComponent
  ],
  imports: [
    CommonModule
  ]
})
export class ContentsModule { }
