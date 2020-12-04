import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { DeckAssignmentService } from '../services/deckAssignment.service';
import { AuthenticationService } from '../services/authentication.service';
import { User } from '../models/user';
import { DeckAssignment } from '../models/deckAssignment';
import { Location } from '@angular/common';
import { Role } from '../models/role';

/**
 * DeckAssignmentList Component
 */
@Component({
  selector: 'app-deck-assignment-list',
  templateUrl: './deck-assignment-list.component.html',
  styleUrls: ['./deck-assignment-list.component.css']
})
export class DeckAssignmentListComponent implements OnInit {
  deckAssignmentsByDeck$: Observable< DeckAssignment[]>;
  deckAssignmentsByDeck: DeckAssignment[];

  deckId: number;
  currentUser: User;

  constructor(private deckAssignmentService: DeckAssignmentService, private avRoute: ActivatedRoute, private _location: Location, private authenticationService: AuthenticationService) { 
    const idParam = 'id';
    if (this.avRoute.snapshot.params[idParam]) {
      this.deckId = this.avRoute.snapshot.params[idParam];
    }
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x); 
  }

    ngOnInit() {
      this.loadDeckAssignmentsByDeck();
    }

    roles: Role[] = [{ key: 'Administrator', value: "Adminisztrátor" },
                      { key: 'Main Lector', value: "Fő Lektor" },
                      { key: 'Lector', value: "Lektor" },
                      { key: 'Main Graphic', value: "Fő Grafikus" },
                      { key: 'Graphic', value: "Grafikus" },
                      { key: 'Main Professional reviewer', value: "Fő Szaklektor" },
                      { key: 'Professional reviewer', value: "Szaklektor" },
                      { key: 'Coordinator', value: "Koordinátor" },
                      { key: 'Card creator', value: "Kártyakészítő" }];

    loadDeckAssignmentsByDeck() {
      this.deckAssignmentsByDeck$ = this.deckAssignmentService.getDeckAssignmentsByDeck(this.deckId);
      
      this.deckAssignmentService.getDeckAssignmentsByDeck(this.deckId).subscribe(res => {
        this.deckAssignmentsByDeck = res;
      });
    }

    get isCoordinator() {
      return this.currentUser && this.currentUser.roles.includes("Coordinator");
    }

    roleToString(roles) {
      if(roles.length > 1) {
        let multipleRoles = "";
        for(var i = 0; i < roles.length; i++) {
          for(let item of this.roles) {
            if(item.key == roles[i]) {
              multipleRoles += item.value + " ";
            }
          }
        }
        return multipleRoles;
      } else {
        for(let item of this.roles) {
          if(item.key == roles) {
            return item.value;
          }
        }
      }      
    }

    delete(deckId, userId) {
      const ans = confirm('Biztosan törölni akarod ezt a hozzárendelést?');
      if (ans) {
        this.deckAssignmentService.deleteDeckAssignment(deckId, userId).subscribe((data) => {
          this.loadDeckAssignmentsByDeck();
          window.location.reload();
        });
      }
    }

    backClicked() {
      this._location.back();
    }
}
