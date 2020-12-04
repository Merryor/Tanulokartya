import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AuthenticationService } from '../services/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { DeckAssignmentService } from '../services/deckAssignment.service';
import { DeckService } from '../services/deck.service';
import { UserService } from '../services/user.service';
import { Deck } from '../models/deck';
import { User } from '../models/user';
import { DeckAssignment } from '../models/deckAssignment';

/**
 * DeckAssignmentForm Component
 */
@Component({
  selector: 'app-deck-assignment-form',
  templateUrl: './deck-assignment-form.component.html',
  styleUrls: ['./deck-assignment-form.component.css']
})
export class DeckAssignmentFormComponent implements OnInit {

    currentUser: User;
    form: FormGroup;
    formLector: string;
    formGraphic: string;
    formProf: string;
    formMainLector: string;
    formMainGraphic: string;
    formMainProf: string;
    errorMessage: any;
    roleName: string;
    submitted = false;

    deckId: number;
    deckName: string;
    deck: Deck;
    user: User;
    deckAssignments: DeckAssignment[];

    lectors: User[];
    graphics: User[];
    profs: User[];
    mainLectors: User[];
    mainGraphics: User[];
    mainProfs: User[];
    lectorFlag: boolean;
    graphicFlag: boolean;
    profFlag: boolean;
    mainLectorFlag: boolean;
    mainGraphicFlag: boolean;
    mainProfFlag: boolean;
    userIds: number[];
    existingAssignmentFlag: boolean;

    constructor(private deckAssignmentService: DeckAssignmentService, private deckService: DeckService, private userService: UserService, private formBuilder: FormBuilder, private avRoute: ActivatedRoute, 
      private router: Router, private authenticationService: AuthenticationService) {
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x); 
        const idParam = 'id';
        this.formLector = 'lector';
        this.formGraphic = 'graphic';
        this.formProf = 'prof';
        this.formMainLector = 'mainLector';
        this.formMainGraphic = 'mainGraphic';
        this.formMainProf = 'mainProf';
        this.existingAssignmentFlag = false;
        if (this.avRoute.snapshot.params[idParam]) {
          this.deckId = this.avRoute.snapshot.params[idParam];
        }

        this.form = this.formBuilder.group(
        {
            lector: [''],
            graphic: [''],
            prof: [''],
            mainLector: [''],
            mainGraphic: [''],
            mainProf: ['']
        })
    }

    ngOnInit() {        
      this.loadDeckAssignments();
      this.loadDeck();
      this.loadUsers();
      this.loadIsLector();    
      this.loadIsGraphic(); 
      this.loadIsProf();   
      this.loadIsMainLector();    
      this.loadIsMainGraphic(); 
      this.loadIsMainProf();
    }

    get isCurrentCoordinator() {
      return this.currentUser && this.currentUser.roles.includes("Coordinator");
    }
    get isCurrentMainLector() {
      return this.currentUser && this.currentUser.roles.includes("Main Lector");
    }
    get isCurrentMainGraphic() {
      return this.currentUser && this.currentUser.roles.includes("Main Graphic");
    }
    get isCurrentMainProf() {
      return this.currentUser && this.currentUser.roles.includes("Main Professional reviewer");
    }
    get isExistingAssignment() {
      return this.existingAssignmentFlag;
    }

    loadIsLector() {
      this.deckAssignmentService.getIsRoleOk(this.deckId, "Lector").subscribe((data) => {
        this.lectorFlag = data;
        return this.lectorFlag;
      });
      return false;
    }
    get isLector() {
      return this.lectorFlag;
    }

    loadIsGraphic() {
      this.deckAssignmentService.getIsRoleOk(this.deckId, "Graphic").subscribe((data) => {
        this.graphicFlag = data;
        return this.graphicFlag;
      });
      return false;
    }
    get isGraphic() {
      return this.graphicFlag;
    }

    loadIsProf() {
      this.deckAssignmentService.getIsRoleOk(this.deckId, "Professional reviewer").subscribe((data) => {
        this.profFlag = data;
        return this.profFlag;
      });
      return false;
    }
    get isProf() {
      return this.profFlag;
    }

    loadIsMainLector() {
      this.deckAssignmentService.getIsRoleOk(this.deckId, "Main Lector").subscribe((data) => {
        this.mainLectorFlag = data;
        return this.mainLectorFlag;
      });
      return false;
    }
    get isMainLector() {
      return this.mainLectorFlag;
    }

    loadIsMainGraphic() {
      this.deckAssignmentService.getIsRoleOk(this.deckId, "Main Graphic").subscribe((data) => {
        this.mainGraphicFlag = data;
        return this.mainGraphicFlag;
      });
      return false;
    }
    get isMainGraphic() {
      return this.mainGraphicFlag;
    }

    loadIsMainProf() {
      this.deckAssignmentService.getIsRoleOk(this.deckId, "Main Professional reviewer").subscribe((data) => {
        this.mainProfFlag = data;
        return this.mainProfFlag;
      });
      return false;
    }
    get isMainProf() {
      return this.mainProfFlag;
    }

    loadDeckAssignments() {
      this.deckAssignmentService.getDeckAssignmentsByDeck(this.deckId).subscribe((data) => {
        this.deckAssignments = data;
        if(this.deckAssignments.find(d=>d.deck.id == this.deckId && d.user.id == this.currentUser.id)) {
          this.existingAssignmentFlag = true;
        }
      });
    }

    loadDeck() {  
      this.deckService.getDeck(this.deckId).subscribe(res => {
        this.deck = res;
        this.deckName = res.name;
      });
    }

    loadUsers() {
      this.roleName="Lector";
      this.userService.getUsersByRole(this.roleName).subscribe((lectors) => {
        this.lectors = lectors;
      });
      this.roleName="Graphic";
      this.userService.getUsersByRole(this.roleName).subscribe((graphics) => {
        this.graphics = graphics;
      });
      this.roleName="Professional reviewer";
      this.userService.getUsersByRole(this.roleName).subscribe((profs) => {
        this.profs = profs;
      });
      this.roleName="Main Lector";
      this.userService.getUsersByRole(this.roleName).subscribe((mainLectors) => {
        this.mainLectors = mainLectors;
      });
      this.roleName="Main Graphic";
      this.userService.getUsersByRole(this.roleName).subscribe((mainGraphics) => {
        this.mainGraphics = mainGraphics;
      });
      this.roleName="Main Professional reviewer";
      this.userService.getUsersByRole(this.roleName).subscribe((mainProfs) => {
        this.mainProfs = mainProfs;
      });
    }

    save() {
      this.submitted = true;

      this.userIds = [this.form.get(this.formLector).value, this.form.get(this.formGraphic).value, this.form.get(this.formProf).value, this.form.get(this.formMainLector).value, this.form.get(this.formMainGraphic).value, this.form.get(this.formMainProf).value];
      for (let i in this.userIds) {
        if(this.userIds[i].toString() !== "") {
          let deckAssignment: DeckAssignment = {
            userId: this.userIds[i].toString(),
            deckId: Number(this.deckId),
            deck: this.deck,
            user: this.lectors.find(l=>l.id == this.userIds[i].toString()) || this.graphics.find(l=>l.id == this.userIds[i].toString()) || this.profs.find(l=>l.id == this.userIds[i].toString()) || this.mainLectors.find(l=>l.id == this.userIds[i].toString()) || this.mainGraphics.find(l=>l.id == this.userIds[i].toString()) || this.mainProfs.find(l=>l.id == this.userIds[i].toString())
          };
                  
          this.deckAssignmentService.addDeckAssignment(deckAssignment)
              .subscribe((data) => {
              });
        }
      }      
      this.router.navigate(['/my-assignment-list/'+this.currentUser.id]);
    }
    
      get lector() { return this.form.get(this.formLector); }
      get graphic() { return this.form.get(this.formGraphic); }
      get prof() { return this.form.get(this.formProf); }
      get mainLector() { return this.form.get(this.formMainLector); }
      get mainGraphic() { return this.form.get(this.formMainGraphic); }
      get mainProf() { return this.form.get(this.formMainProf); }
}
