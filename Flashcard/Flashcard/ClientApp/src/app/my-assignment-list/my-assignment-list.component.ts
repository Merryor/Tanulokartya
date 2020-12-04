import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { DeckAssignmentService } from '../services/deckAssignment.service';
import { AuthenticationService } from '../services/authentication.service';
import { User } from '../models/user';
import { DeckAssignment } from '../models/deckAssignment';

/**
 * MyAssignmentList Component
 */
@Component({
  selector: 'app-my-assignment-list',
  templateUrl: './my-assignment-list.component.html',
  styleUrls: ['./my-assignment-list.component.css']
})
export class MyAssignmentListComponent implements OnInit {
  deckAssignments$: Observable<DeckAssignment[]>;  
  deckAssignments: DeckAssignment[];
  currentUser: User;
  deckAuthors: User[];
  deckAuthor: User;

  constructor(private deckAssignmentService: DeckAssignmentService, private authenticationService: AuthenticationService) { 
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x); 
  }

    ngOnInit() {
      this.loadDeckAssignments();
    }

    get isMainPerson() {
      return this.currentUser && (this.currentUser.roles.includes("Main Lector") || this.currentUser.roles.includes("Main Graphic") || this.currentUser.roles.includes("Main Professional reviewer"));
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
}
