import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { DeckService } from '../services/deck.service';
import { Deck } from '../models/deck';

/**
 * ActivatedDeckList Component
 */
@Component({
  selector: 'app-activated-deck-list',
  templateUrl: './activated-deck-list.component.html',
  styleUrls: ['./activated-deck-list.component.css']
})
export class ActivatedDeckListComponent implements OnInit {
  decks$: Observable<Deck[]>;  
  decks: Deck[];
  searchText: string;

  constructor(private deckService: DeckService) { }

    ngOnInit() {
      this.loadDecks();
    }

    loadDecks() {
        this.decks$ = this.deckService.getDecks();
        this.deckService.getDecks().subscribe(res => {
          this.decks = res;
        });
    }

    moduleToString(moduleIndex) {
      const moduleString: string[] = ["(E) iskolaelőkészítő", "(A) 1-2. osztály", "(B) 3-4. osztály", "(C) 5-6. osztály", "(D) 7-8. osztály", "(F) 9-10. osztály", "(G) 11-12. osztály", "(J) joker"];
      return moduleString[moduleIndex];
    }
}
