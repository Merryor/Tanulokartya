import { Component } from '@angular/core';
import { User } from './models/user';
import { Router, NavigationEnd } from '@angular/router';
import { AuthenticationService } from './services/authentication.service';

declare var gtag;

/**
 * AppComponent
 */
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  currentUser: User;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService
    ) {
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);

        this.router.events.subscribe(event => {
          // Google Analytics
          if (event instanceof NavigationEnd) {
            gtag('config', 'UA-179966700-1', {
              'page_path': event.urlAfterRedirects
            });
          }  
        });
      }

    get isAdmin() {
      return this.currentUser && this.currentUser.roles.includes("Administrator");
    }

    get isCardCreator() {
      return this.currentUser && this.currentUser.roles.includes("Card creator");
    }

    get isAssigned() {
      return this.currentUser.roles.includes("Lector") || this.currentUser.roles.includes("Graphic") || this.currentUser.roles.includes("Professional reviewer");
    } 

    get isMain() {
      return this.currentUser.roles.includes("Main Lector") || this.currentUser.roles.includes("Main Graphic") || this.currentUser.roles.includes("Main Professional reviewer");
    } 

    logout() {
        this.authenticationService.logout();
        this.router.navigate(['/']);
    }
}
