import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { CardService } from '../services/card.service';
import { DeckService } from '../services/deck.service';
import { AuthenticationService } from '../services/authentication.service';
import { Card } from '../models/card';
import { User } from '../models/user';
import { environment } from 'src/environments/environment';

/**
 * CardList Component
 */
@Component({
  selector: 'app-card-list',
  templateUrl: './card-list.component.html',
  styleUrls: ['./card-list.component.css']
})
export class CardListComponent implements OnInit {
  cards$: Observable<Card[]>;
  deckId: number;
  deckName: string;
  currentUser: User;  
  deckAuthorId: string;  
  deckStatus: number;  
  deckActivated: boolean;  
  cards: Card[];

  constructor(private cardService: CardService, private deckService: DeckService,
    private router: Router, private avRoute: ActivatedRoute, private authenticationService: AuthenticationService) { 
    const idParam = 'id';
    if (this.avRoute.snapshot.params[idParam]) {
      this.deckId = this.avRoute.snapshot.params[idParam];
    }
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x); 
  }

    ngOnInit() {
      this.loadCards();
      this.loadDeck();
    }

    loadCards() {
      this.cards$ = this.cardService.getCardsByDeck(this.deckId);
      
      this.cardService.getCardsByDeck(this.deckId).subscribe(res => {
        this.cards = res;
      });
    }

    loadDeck() {  
      this.deckService.getDeck(this.deckId).subscribe(res => {
        this.deckAuthorId = res.applicationUser.id;
        this.deckStatus = res.status;
        this.deckName = res.name;
        this.deckActivated = res.activated;
      });
    }

    isPicture(pic: string) {
      return pic != "";
    }

    get isAdmin() {
      return this.currentUser && this.currentUser.roles.includes("Administrator");
    }

    get isCardCreator() {
      return this.currentUser && this.currentUser.roles.includes("Card creator");
    }

    get isStatusInitial() {
      return this.deckStatus == 0;
    }

    get isActivated() {
      return this.deckActivated == true && this.deckStatus == 4;
    }

    get isAuthor() {
      return this.currentUser && this.deckAuthorId == this.currentUser.id;
    }

    delete(id) {
      const ans = confirm('Biztosan törölni akarod ezt a kártyát? id: ' + id);
      if (ans) {
        this.cardService.deleteCard(id).subscribe((data) => {
          this.loadCards();
        });
      }
    }

    public createImgPath = (serverPath: string) => {
        return `${environment.apiBaseUrl}/${serverPath}`;
    }
    
    backClicked() {      
      this.router.navigate(['/deck-list']);
    }
}
