import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { UserService } from '../services/user.service';
import { User } from '../models/user';
import { Role } from '../models/role';

/**
 * UserList Component
 */
@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  users$: Observable<User[]>;
  searchText: string;

  constructor(private userService: UserService) { }

    ngOnInit() {
      this.loadUsers();
    }

    roles: Role[] = [{ key: 'Administrator', value: "Adminisztrátor" },
                      { key: 'Main Lector', value: "Fő Lektor" },
                      { key: 'Lector', value: "Lektor" },
                      { key: 'Main Graphic', value: "Fő Grafikus" },
                      { key: 'Graphic', value: "Grafikus" },
                      { key: 'Main Professional reviewer', value: "Fő Szaklektor" },
                      { key: 'Professional reviewer', value: "Szaklektor" },
                      { key: 'Coordinator', value: "Koordinátor" },
                      { key: 'Card creator', value: "Kártyakészítő" }];

    loadUsers() {
      this.users$ = this.userService.getUsers();
    }

    moduleToString(moduleIndex) {
      const moduleString: string[] = ["(E) iskolaelőkészítő", "(A) 1-2. osztály", "(B) 3-4. osztály", "(C) 5-6. osztály", "(D) 7-8. osztály", "(F) 9-10. osztály", "(G) 11-12. osztály", "(J) joker"];
      return moduleString[moduleIndex];
    }

    activateToString(activation) {
      if(activation == true) {
        return "Igen";
      } else {
        return "Nem";
      }
    }

    roleToString(roles) {
      if(roles.length > 1) {
        let multipleRoles = "";
        for(var i = 0; i < roles.length; i++) {
          for(let item of this.roles) {
            if(item.key == roles[i]) {
              multipleRoles += item.value + " ";
            }
          }
        }
        return multipleRoles;
      } else {
        for(let item of this.roles) {
          if(item.key == roles) {
            return item.value;
          }
        }
      }      
    }

    deactivate(id) {
      const ans = confirm('Biztosan deaktiválod ezt a felhasználót?');
      if (ans) {
        this.userService.deactivateUser(id).subscribe((data) => {
          this.loadUsers();
        });
      }
    }
}
