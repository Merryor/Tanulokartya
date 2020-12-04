import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { DeckService } from '../services/deck.service';
import { DeckAssignmentService } from '../services/deckAssignment.service';
import { UserService } from '../services/user.service';
import { AuthenticationService } from '../services/authentication.service';
import { Deck } from '../models/deck';
import { User } from '../models/user';
import { DeckAssignment } from '../models/deckAssignment';

/**
 * AssignedDeckList Component
 */
@Component({
  selector: 'app-assigned-deck-list',
  templateUrl: './assigned-deck-list.component.html',
  styleUrls: ['./assigned-deck-list.component.css']
})
export class AssignedDeckListComponent implements OnInit {
  deckAssignments$: Observable<DeckAssignment[]>;  
  deckAssignments: DeckAssignment[];
  decks$: Observable<Deck[]>;  
  decks: Deck[];
  currentUser: User;
  deckAuthors: User[];
  deckAuthor: User;

  constructor(private deckService: DeckService, private deckAssignmentService: DeckAssignmentService, private authenticationService: AuthenticationService, private userService: UserService) { 
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x); 
  }

    ngOnInit() {
      this.loadDeckAssignments();
    }

    get isLector() {
      return this.currentUser && (this.currentUser.roles.includes("Lector") || this.currentUser.roles.includes("Main Lector"));
    }

    get isGraphic() {
      return this.currentUser && (this.currentUser.roles.includes("Graphic") || this.currentUser.roles.includes("Main Graphic"));
    }

    get isProf() {
      return this.currentUser && (this.currentUser.roles.includes("Professional reviewer") || this.currentUser.roles.includes("Main Professional reviewer"));
    }

    isEditDisabled(status) {
     if(this.currentUser.roles.includes("Card creator") && status == 0 ||
        this.currentUser.roles.includes("Lector") && status == 0 ||
        this.currentUser.roles.includes("Main Lector") && status == 0 ||
        this.currentUser.roles.includes("Graphic") && status == 1 ||
        this.currentUser.roles.includes("Main Graphic") && status == 1 ||
        this.currentUser.roles.includes("Professional reviewer") && status == 2 ||
        this.currentUser.roles.includes("Main Professional reviewer") && status == 2 ||
        this.currentUser.roles.includes("Administrator") && status == 3) {
          return false; // enabled
        }
        return true; // disabled
    }

    loadDeckAssignments() {
        this.deckAssignments$ = this.deckAssignmentService.GetDeckAssignmentsByUser(this.currentUser.id);
        this.deckAssignmentService.GetDeckAssignmentsByUser(this.currentUser.id).subscribe(res => {
          this.deckAssignments = res;
        });
    }

    moduleToString(moduleIndex) {
      const moduleString: string[] = ["(E) iskolaelőkészítő", "(A) 1-2. osztály", "(B) 3-4. osztály", "(C) 5-6. osztály", "(D) 7-8. osztály", "(F) 9-10. osztály", "(G) 11-12. osztály", "(J) joker"];
      return moduleString[moduleIndex];
    }

    statusToString(statusIndex) {
      const statusString: string[] = ["Kezdeti", "Lektorálva", "Grafikálva", "Szaklektorálva", "Jóváhagyva"];
      return statusString[statusIndex];
    }

    activateToString(activation) {
      if(activation == true) {
        return "Igen";
      } else {
        return "Nem";
      }
    }

    stateUpdate(id, state) {
      this.deckService.updateStateDeck(id,state).subscribe((data) => {
        this.loadDeckAssignments();
      });
    }
}
