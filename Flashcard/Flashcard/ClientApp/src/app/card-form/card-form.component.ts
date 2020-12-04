import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CardService } from '../services/card.service';
import { DeckService } from '../services/deck.service';
import { AuthenticationService } from '../services/authentication.service';
import { Card } from '../models/card';
import { Deck } from '../models/deck';
import { User } from '../models/user';
import { environment } from 'src/environments/environment';
import { maxLength } from '../helpers/maxlength.validator';
import { AsyncSubject, Subject } from 'rxjs';

/**
 * CardForm Component
 */
@Component({
  selector: 'app-card-form',
  templateUrl: './card-form.component.html',
  styleUrls: ['./card-form.component.css']
})
export class CardFormComponent implements OnInit {

    form: FormGroup;
    formType: string;
    formQuestion_text: string;
    formQuestion_picture: string;
    formAnswer_text: string;
    formAnswer_picture: string;
    id: number;
    errorMessage: any;
    error = '';

    deckId: number;
    deck: Deck;
    deckStatus: number;
    deckAuthorId: string;
    cardNumber: number;
    
    cardId: number;
    actionType: string;
    existingCard: Card;
    submitted = false;
    currentUser: User;

    private editorSubject: Subject<any> = new AsyncSubject();

    // File upload
    public response: {dbPath: ''};
    public response_answer: {dbPath: ''};

    constructor(private cardService: CardService, private deckService: DeckService, private formBuilder: FormBuilder, private avRoute: ActivatedRoute, 
      private router: Router, private authenticationService: AuthenticationService) {
        const idParam = 'id';
        const cardIdParam = 'cardId';
        this.actionType = 'Új';
        this.formType = 'type';
        this.formQuestion_text = 'question_text';
        this.formQuestion_picture = 'question_picture';
        this.formAnswer_text = 'answer_text';
        this.formAnswer_picture = 'answer_picture';
        
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
   
        if (this.avRoute.snapshot.params[idParam]) {
          if(this.router.url.includes("add")) {
            this.deckId = this.avRoute.snapshot.params[idParam];  
            this.actionType = "Új";        
          } else {  
            this.deckId = this.avRoute.snapshot.params[idParam];
            this.cardId = this.avRoute.snapshot.params[cardIdParam];
          }
        }

        this.form = this.formBuilder.group(
        {
            id: 0,
            type: ['', [Validators.maxLength(200), Validators.required]],
            question_text: ['', Validators.required],
            answer_text: ['', Validators.required],
        },
        {
          validator:  [maxLength('question_text', 200), maxLength('answer_text', 200)]
        });
  }

    ngOnInit() {  
      if(this.deckId !== undefined) {
        this.loadCountCard();
        this.loadDeck();
      }      
      if (this.cardId !== undefined) {
        this.loadExistingCard();
      }
    }

    loadExistingCard() {      
      this.actionType = 'Szerkesztendő';
      this.cardService.getCard(this.cardId)
        .subscribe(data => (
          this.existingCard = data,
          this.form.controls[this.formType].setValue(data.type),
          this.form.controls[this.formQuestion_text].setValue(data.question_text),
          this.form.controls[this.formAnswer_text].setValue(data.answer_text)
        ));
    }
    
    loadCountCard() {
      this.cardService.getCountCard(this.deckId).subscribe((data) => {
        this.cardNumber = data;
      });
    }

    loadDeck() {
      this.deckService.getDeck(this.deckId).subscribe(res => {
        this.deck = res;
        this.deckStatus = res.status;
        this.deckAuthorId = res.applicationUser.id;
      });
    }

    isTextEditable() {
      if((this.currentUser.roles.includes("Card creator") && this.deckStatus == 0 && this.deckAuthorId === this.currentUser.id) ||
        this.currentUser.roles.includes("Lector") && this.deckStatus == 0 ||
        this.currentUser.roles.includes("Main Lector") && this.deckStatus == 0 ||
        this.currentUser.roles.includes("Professional reviewer") && this.deckStatus == 2 ||
        this.currentUser.roles.includes("Main Professional reviewer") && this.deckStatus == 2) {
          return true; // enabled
        }
        return false; // disabled
    }

    isPictureEditable() {
      if((this.currentUser.roles.includes("Card creator") && this.deckStatus == 0) ||
        (this.currentUser.roles.includes("Graphic") && this.deckStatus == 1) ||
        (this.currentUser.roles.includes("Main Graphic") && this.deckStatus == 1) ||
        (this.currentUser.roles.includes("Professional reviewer") && this.deckStatus == 2) ||
        (this.currentUser.roles.includes("Main Professional reviewer") && this.deckStatus == 2)) {
          return true; // enabled
        }
        return false; // disabled
    }

    save() {        
      this.submitted = true;
      if (this.form.invalid) {
        return;
      }
      if(this.actionType === "Új") {
        let card: Card = {
          id: 1,
          type: this.form.get(this.formType).value,
          card_number: Number(this.cardNumber),
          question_text: this.form.get(this.formQuestion_text).value,
          question_picture: "",
          answer_text: this.form.get(this.formAnswer_text).value,
          answer_picture: "",
          deckId: Number(this.deckId),
          deck: this.deck
        };
        if(this.response !== undefined) {
          card.question_picture = this.response.dbPath;
        }
        if(this.response_answer !== undefined) {
          card.answer_picture = this.response_answer.dbPath;
        }
        
        this.cardService.addCard(card)
            .subscribe((data) => {
              this.router.navigate(['/card-list/', this.deckId]);
            });
      }

      if(this.actionType === 'Szerkesztendő') {
        let card: Card = {
          id: this.existingCard.id,
          type: this.form.get(this.formType).value,
          card_number: this.existingCard.card_number,
          question_text: this.form.get(this.formQuestion_text).value,
          question_picture: this.QuestionPicture,
          answer_text: this.form.get(this.formAnswer_text).value,
          answer_picture: this.AnswerPicture,
          deckId: this.existingCard.deckId,
          deck: this.existingCard.deck
        };
        if(this.response !== undefined) {
          card.question_picture = this.response.dbPath;
        }
        if(this.response_answer !== undefined) {
          card.answer_picture = this.response_answer.dbPath;
        }

        this.cardService.updateCard(card.id, card)
        .subscribe((data) => {
          this.router.navigate(['/assigned-card-list/', this.existingCard.deckId]);
        });
      }        
    }
    
      get type() { return this.form.get(this.formType); }
      get question_text() { return this.form.get(this.formQuestion_text); }
      get question_picture() { return this.form.get(this.formQuestion_picture); }
      get answer_text() { return this.form.get(this.formAnswer_text); }
      get answer_picture() { return this.form.get(this.formAnswer_picture); }

      get isQuestionPicture() {
        return this.existingCard.question_picture !== "";
      }
      get QuestionPicture() {
        if(this.existingCard.question_picture !== "") {
          return this.existingCard.question_picture;
        }
        else return "";
      }
      get isAnswerPicture() {
        return this.existingCard.answer_picture !== "";
      }
      get AnswerPicture() {
        if(this.existingCard.answer_picture !== "") {
          return this.existingCard.answer_picture;
        }
        else return "";
      }

      isPicture(pic: string) {
        return pic != "";
      }

      // File upload
      public uploadFinished = (event) => {
        this.response = event;
      }

      public uploadFinishedAnswer = (event) => {
        this.response_answer = event;
      }

      public createImgPath = (serverPath: string) => {
          return `${environment.apiBaseUrl}/${serverPath}`;
      }

      handleEditorInit(e) {
        this.editorSubject.next(e.editor);
        this.editorSubject.complete();
      }

      delete(id, type) {
        const ans = confirm('Biztosan törölni akarod az eddigi képet?');
        if (ans) {
          this.cardService.deleteCardPicture(id, type).subscribe((data) => {
            this.loadExistingCard();
          });
        }
      }
      
      backClicked() {
        this.router.navigate(['/assigned-card-list',this.deckId]);
      }
}
