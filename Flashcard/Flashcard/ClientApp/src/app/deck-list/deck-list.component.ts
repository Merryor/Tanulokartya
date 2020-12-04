import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { DeckService } from '../services/deck.service';
import { AuthenticationService } from '../services/authentication.service';
import { Deck } from '../models/deck';
import { User } from '../models/user';

/**
 * DeckList Component
 */
@Component({
  selector: 'app-deck-list',
  templateUrl: './deck-list.component.html',
  styleUrls: ['./deck-list.component.css']
})
export class DeckListComponent implements OnInit {
  decks$: Observable<Deck[]>;  
  decks: Deck[];
  currentUser: User;
  deckAuthors: User[];
  deckAuthor: User;
  searchText: string;
  activated: boolean;

  constructor(private deckService: DeckService, private authenticationService: AuthenticationService) { 
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x); 
  }

    ngOnInit() {
      this.loadDecks();
    }

    get isCardCreator() {
      return this.currentUser && this.currentUser.roles.includes("Card creator");
    }

    get isAdmin() {
      return this.currentUser && this.currentUser.roles.includes("Administrator");
    }

    get isCoordinator() {
      return this.currentUser && this.currentUser.roles.includes("Coordinator");
    }

    get isCoordinatorOrMainPerson() {
      return this.currentUser && (this.currentUser.roles.includes("Coordinator") || this.currentUser.roles.includes("Main Lector") || this.currentUser.roles.includes("Main Graphic") || this.currentUser.roles.includes("Main Professional reviewer"));
    }

    loadDecks() {
        this.decks$ = this.deckService.getAllDecks();
        this.deckService.getAllDecks().subscribe(res => {
          this.decks = res;
        });
    }

    deckFilterByActivate() {
      this.activated = !this.activated;
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

    activation(id) {
        this.deckService.updateActivateDeck(id).subscribe((data) => {
          this.loadDecks();
        });
    }

    stateUpdate(id, state) {
      this.deckService.updateStateDeck(id,state).subscribe((data) => {
        this.loadDecks();
      });
    }

    initial_state(id) {
      const ans = confirm('Biztosan kezdeti állapotra rakod ezt a csomagot?');
      if (ans) {
        this.deckService.updateStateDeck(id, 0).subscribe((data) => {
          this.loadDecks();
        });
      }
    }
}
