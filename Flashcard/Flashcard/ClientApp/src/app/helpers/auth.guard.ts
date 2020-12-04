import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';

/**
 * AuthGuard class responsible for authorization
 */
@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    currentId: string;
    currentRoles: Array<string>;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const currentUser = this.authenticationService.currentUserValue;
        if (currentUser) {
            this.currentRoles = currentUser.roles;

            if(!currentUser.roles.includes("Administrator")) {
                // User can only access their own profile
                const idParam = 'id';
                if(route.url[0].path.includes("user-form")) {
                    if(route.params[idParam]) {
                        this.currentId = route.params[idParam];
                        if(currentUser.id != this.currentId) {
                            this.router.navigate(['/']);
                            return false;
                        }
                    }
                }                
            }
            
            // Check if route is restricted by role
            if (route.data.roles && (!route.data.roles.includes(this.currentRoles[0]) && !route.data.roles.includes(this.currentRoles[1]))) {
                // Role not authorised, so redirect to home page
                this.router.navigate(['/']);
                return false;
            }

            // Authorised, so return true
            return true;
        }

        // Not logged in, so redirect to login page with the return url
        this.router.navigate(['/'], { queryParams: { returnUrl: state.url } });
        return false;
    }
}