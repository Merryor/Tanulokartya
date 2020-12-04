import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { DeckService } from '../services/deck.service';
import { AuthenticationService } from '../services/authentication.service';
import { Deck } from '../models/deck';
import { Enum } from '../models/enum';
import { User } from '../models/user';

/**
 * DeckForm Component
 */
@Component({
    selector: 'app-deck-form',
    templateUrl: './deck-form.component.html',
    styleUrls: ['./deck-form.component.css']
})
export class DeckFormComponent implements OnInit {
    form: FormGroup;
    formName: string;
    formContent: string;
    formModule: string;
    id: number;
    errorMessage: any;
    currentUser: User;

    moduleNumber: number;
    deckNumber: number;
    
    actionType: string;
    existingDeck: Deck;
    submitted = false;

    modules: Enum[] = [{ key: 0, value: 'E - iskolaelőkészítő' },
                    { key: 1, value: 'A - 1-2. osztály' },
                    { key: 2, value: 'B - 3-4. osztály' },
                    { key: 3, value: 'C - 5-6. osztály' },
                    { key: 4, value: 'D - 7-8. osztály' },
                    { key: 5, value: 'F - 9-10. osztály' },
                    { key: 6, value: 'G - 11-12. osztály' },
                    { key: 7, value: 'J - joker' }];
    
    constructor(private deckService: DeckService, private formBuilder: FormBuilder, private avRoute: ActivatedRoute, private _location: Location, private router: Router, private authenticationService: AuthenticationService) {
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x); 
        const idParam = 'id';
        this.actionType = 'Új';
        this.formName = 'name';
        this.formContent = 'content';
        this.formModule = 'module';
        if (this.avRoute.snapshot.params[idParam]) {
        this.id = this.avRoute.snapshot.params[idParam];
        }

        this.form = this.formBuilder.group(
        {
            id: 0,
            name: ['', [Validators.maxLength(200), Validators.required]],
            content: ['', [Validators.maxLength(200), Validators.required]],
            module: ['', [Validators.required]]
        }
        )
    }

    ngOnInit() {
        if (this.id !== undefined) {
            this.actionType = 'Szerkesztendő';
            this.deckService.getDeck(this.id)
              .subscribe(data => (
                this.existingDeck = data,
                this.form.controls[this.formName].setValue(data.name),
                this.form.controls[this.formContent].setValue(data.content),
                this.form.controls[this.formModule].setValue(data.module)
              ));
          }
    }

    save() {
        this.submitted = true;
        if (this.form.invalid) {
            return;
        }
        // New
        if(this.actionType === "Új") {
            this.moduleNumber = this.form.get(this.formModule).value;        
            this.deckService.getCountDeck(this.moduleNumber).subscribe((data) => {
                this.deckNumber = data;
                let deck: Deck = {
                    id: 10,
                    name: this.form.get(this.formName).value,
                    content: this.form.get(this.formContent).value,
                    module: Number(this.form.get(this.formModule).value),
                    deck_number : this.deckNumber,
                    activated: false,
                    status: 0,
                    applicationUserId: this.currentUser.id,
                    applicationUser: this.currentUser
                };
    
                this.deckService.addDeck(deck).subscribe((data) => {
                    this.router.navigate(['/assigned-deck-list']);
                });
              });
        }
        // Edit
        if(this.actionType === 'Szerkesztendő') {
            let deck: Deck = {
                id: this.existingDeck.id,
                name: this.form.get(this.formName).value,
                content: this.form.get(this.formContent).value,
                module: Number(this.form.get(this.formModule).value),
                deck_number : this.existingDeck.deck_number,
                activated: this.existingDeck.activated,
                status: this.existingDeck.status,
                applicationUserId: this.currentUser.id,
                applicationUser: this.currentUser
            };

            this.deckService.updateDeck(deck.id, deck)
              .subscribe((data) => {
                this._location.back();
              });
        }       
    }

    get name() { return this.form.get(this.formName); }
    get content() { return this.form.get(this.formContent); }
    get module() { return this.form.get(this.formModule); }
}