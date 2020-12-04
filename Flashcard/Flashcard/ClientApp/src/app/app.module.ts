import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppComponent } from './app.component';
import { LoginPageComponent } from './login-page/login-page.component';
import { DeckListComponent } from './deck-list/deck-list.component';
import { AssignedDeckListComponent } from './assigned-deck-list/assigned-deck-list.component';
import { MyDeckListComponent } from './my-deck-list/my-deck-list.component';
import { MyAssignmentListComponent } from './my-assignment-list/my-assignment-list.component';
import { DeckFormComponent } from './deck-form/deck-form.component';
import { CardListComponent } from './card-list/card-list.component';
import { AssignedCardListComponent } from './assigned-card-list/assigned-card-list.component';
import { CardFormComponent } from './card-form/card-form.component';
import { CardElementComponent } from './card-element/card-element.component';
import { UserListComponent } from './user-list/user-list.component';
import { UserFormComponent } from './user-form/user-form.component';
import { UploadComponent } from './upload/upload.component';
import { DeckAssignmentListComponent } from './deck-assignment-list/deck-assignment-list.component';
import { DeckAssignmentFormComponent } from './deck-assignment-form/deck-assignment-form.component';
import { ActivatedDeckListComponent } from './activated-deck-list/activated-deck-list.component';

import { ErrorInterceptor } from './helpers/error.interceptor';
import { JwtInterceptor } from './helpers/jwt.interceptor';
import { appRoutingModule } from './app.routing';
import { FilterPipe } from './helpers/filter.pipe';
import { FilterBoolPipe } from './helpers/filter-bool.pipe';
import { EditorModule } from '@tinymce/tinymce-angular';

@NgModule({
  declarations: [
    AppComponent,
    DeckListComponent,
    AssignedDeckListComponent,
    DeckFormComponent,
    CardListComponent,
    AssignedCardListComponent,
    CardFormComponent,
    CardElementComponent,    
    UserListComponent, 
    UserFormComponent,
    UploadComponent,
    LoginPageComponent,
    DeckAssignmentFormComponent,
    DeckAssignmentListComponent,
    MyDeckListComponent,
    MyAssignmentListComponent,
    ActivatedDeckListComponent,
    FilterPipe,
    FilterBoolPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    appRoutingModule,
    EditorModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
