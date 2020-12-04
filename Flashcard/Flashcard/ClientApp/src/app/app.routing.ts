import { Routes, RouterModule } from '@angular/router';

import { LoginPageComponent } from './login-page/login-page.component';
import { DeckListComponent } from './deck-list/deck-list.component';
import { AssignedDeckListComponent } from './assigned-deck-list/assigned-deck-list.component';
import { DeckFormComponent } from './deck-form/deck-form.component';
import { MyDeckListComponent } from './my-deck-list/my-deck-list.component';
import { MyAssignmentListComponent } from './my-assignment-list/my-assignment-list.component';
import { CardListComponent } from './card-list/card-list.component';
import { AssignedCardListComponent } from './assigned-card-list/assigned-card-list.component';
import { CardFormComponent } from './card-form/card-form.component';
import { CardElementComponent } from './card-element/card-element.component';
import { UserListComponent } from './user-list/user-list.component';
import { UserFormComponent } from './user-form/user-form.component';
import { DeckAssignmentFormComponent } from './deck-assignment-form/deck-assignment-form.component';
import { ActivatedDeckListComponent } from './activated-deck-list/activated-deck-list.component';

import { AuthGuard } from './helpers/auth.guard';

/**
 * App routing
 */
const routes: Routes = [
    { path: '', component: LoginPageComponent, pathMatch: 'full' },
        { path: 'deck-list', component: DeckListComponent, canActivate: [AuthGuard] },
        { path: 'activated-deck-list', component: ActivatedDeckListComponent },
        { path: 'assigned-deck-list', component: AssignedDeckListComponent, canActivate: [AuthGuard], data: { roles: ['Lector', 'Graphic', 'Professional reviewer', 'Main Lector', 'Main Graphic', 'Main Professional reviewer']} },
        { path: 'my-deck-list/:id', component: MyDeckListComponent, canActivate: [AuthGuard], data: { roles: "Card creator"} },
        { path: 'my-assignment-list/:id', component: MyAssignmentListComponent, canActivate: [AuthGuard], data: { roles: ['Main Lector', 'Main Graphic', 'Main Professional reviewer']} },
        { path: 'deck-form/add', component: DeckFormComponent, canActivate: [AuthGuard], data: { roles: "Card creator"} },
        { path: 'deck-form/edit/:id', component: DeckFormComponent, canActivate: [AuthGuard], data: { roles: ['Card creator', 'Lector', 'Professional reviewer', 'Main Lector', 'Main Professional reviewer']} },
        { path: 'card-list/:id', component: CardListComponent, canActivate: [AuthGuard] },
        { path: 'assigned-card-list/:id', component: AssignedCardListComponent, canActivate: [AuthGuard], data: { roles: ['Card creator', 'Lector', 'Graphic','Professional reviewer', 'Main Lector', 'Main Graphic', 'Main Professional reviewer']} },
        { path: 'card-form/add/:id', component: CardFormComponent, canActivate: [AuthGuard], data: { roles: "Card creator"} },
        { path: 'card-form/edit/:id/:cardId', component: CardFormComponent, canActivate: [AuthGuard], data: { roles: ['Card creator', 'Lector', 'Graphic', 'Professional reviewer', 'Main Lector', 'Main Graphic', 'Main Professional reviewer']} },
        { path: 'card-element/:id', component: CardElementComponent, canActivate: [AuthGuard] },
        { path: 'user-list', component: UserListComponent, canActivate: [AuthGuard], data: { roles: "Administrator"} },
        { path: 'user-form/add', component: UserFormComponent, canActivate: [AuthGuard], data: { roles: "Administrator"} },
        { path: 'user-form/edit/:id', component: UserFormComponent, canActivate: [AuthGuard] },
        { path: 'deck-assignment-form/:id', component: DeckAssignmentFormComponent, canActivate: [AuthGuard], data: { roles: ['Coordinator', 'Main Lector', 'Main Graphic', 'Main Professional reviewer']} },
    { path: '**', redirectTo: '/', pathMatch: 'full' }
];

export const appRoutingModule = RouterModule.forRoot(routes);