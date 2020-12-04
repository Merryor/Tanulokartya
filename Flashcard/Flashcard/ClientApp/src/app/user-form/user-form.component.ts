import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from '../services/user.service';
import { AuthenticationService } from '../services/authentication.service';
import { User } from '../models/user';
import { Enum } from '../models/enum';
import { Role } from '../models/role';
import { Location } from '@angular/common';
import { MustMatch } from '../helpers/must-match.validator';

/**
 * UserForm Component
 */
@Component({
  selector: 'app-user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent implements OnInit {
    currentUser: User;  
    form: FormGroup;
    plusForm: FormGroup;
    actionType: string;
    formName: string;
    formPhone: string;
    formEmail: string;
    formPassword: string;
    formPasswordAgain: string;
    formWorkplace: string;
    formCreate_module: string;
    formWill_create_module: string;
    formRole: string;
    formPlusRole: string;
    id: string;
    errorMessage: any;
    error = '';
    loading = false;
    existingUser: User;
    submitted = false;
    plusRole: string;

    modules: Enum[] = [{ key: 0, value: 'E - iskolaelőkészítő' },
                    { key: 1, value: 'A - 1-2. osztály' },
                    { key: 2, value: 'B - 3-4. osztály' },
                    { key: 3, value: 'C - 5-6. osztály' },
                    { key: 4, value: 'D - 7-8. osztály' },
                    { key: 5, value: 'F - 9-10. osztály' },
                    { key: 6, value: 'G - 11-12. osztály' },
                    { key: 7, value: 'J - joker' }];

    roles: Role[] = [{ key: 'Administrator', value: "Adminisztrátor" },
                    { key: 'Main Lector', value: "Fő Lektor" },
                    { key: 'Lector', value: "Lektor" },
                    { key: 'Main Graphic', value: "Fő Grafikus" },
                    { key: 'Graphic', value: "Grafikus" },
                    { key: 'Main Professional reviewer', value: "Fő Szaklektor" },
                    { key: 'Professional reviewer', value: "Szaklektor" },
                    { key: 'Coordinator', value: "Koordinátor" },
                    { key: 'Card creator', value: "Kártyakészítő" }];

    rolesAdmin: Role[] = [{ key: 'Main Lector', value: "Fő Lektor" },
                    { key: 'Lector', value: "Lektor" },
                    { key: 'Main Graphic', value: "Fő Grafikus" },
                    { key: 'Graphic', value: "Grafikus" },
                    { key: 'Main Professional reviewer', value: "Fő Szaklektor" },
                    { key: 'Professional reviewer', value: "Szaklektor" },
                    { key: 'Coordinator', value: "Koordinátor" },
                    { key: 'Card creator', value: "Kártyakészítő" }];

    rolesCardCreator: Role[] = [{ key: 'Card creator', value: "Kártyakészítő" }];


    constructor(private userService: UserService, private formBuilder: FormBuilder, private avRoute: ActivatedRoute, private router: Router, private _location: Location, private authenticationService: AuthenticationService) {
        const idParam = 'id';
        this.actionType = 'Új';
        this.formName = 'name';
        this.formPhone = 'phone';
        this.formEmail = 'email';
        this.formPassword = 'password';
        this.formPasswordAgain = 'password_again';
        this.formWorkplace = 'workplace';
        this.formCreate_module = 'create_module';
        this.formWill_create_module = 'will_create_module';
        this.formRole = 'role';
        this.formPlusRole = 'plus_role';
        if (this.avRoute.snapshot.params[idParam]) {
        this.id = this.avRoute.snapshot.params[idParam];
        }
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x); 

        this.form = this.formBuilder.group(
        {
            id: "0",
            name: ['', [Validators.maxLength(200), Validators.required]],
            phone: ['', [Validators.required]],
            email: ['', [Validators.email, Validators.required]],
            password: ['', [Validators.required, Validators.minLength(5)]],
            password_again: ['', [Validators.required, Validators.minLength(5)]],
            workplace: ['', [Validators.maxLength(200), Validators.required]],
            create_module: ['', [Validators.required]],
            will_create_module: ['', [Validators.required]],
            role: ['', [Validators.required]],
        }, {
          validator: MustMatch('password', 'password_again')
      });

      this.plusForm = this.formBuilder.group({
        plus_role: ['', [Validators.required]],
      });
    }

    ngOnInit() {
        this.loadUser();
    }

    loadUser() {
      if (this.id !== undefined) {
        this.actionType = 'Szerkesztendő';
        this.userService.getUser(this.id)
          .subscribe(data => (
            this.existingUser = data,
            this.form.controls[this.formName].setValue(data.name),
            this.form.controls[this.formPhone].setValue(data.phone),
            this.form.controls[this.formEmail].setValue(data.email),
            this.form.controls[this.formWorkplace].setValue(data.workplace),
            this.form.controls[this.formCreate_module].setValue(data.create_module),
            this.form.controls[this.formWill_create_module].setValue(data.will_create_module),
            this.form.controls[this.formRole].setValue(data.roles)
          ));
      }
    }

    get isAdmin() {
      return this.currentUser && this.currentUser.roles.includes("Administrator");
    }

    get isExistingUserAdmin() {
      return this.existingUser && this.existingUser.roles.includes("Administrator");
    }

    get isExistingUserWithProperRole() {
      return this.existingUser && (this.existingUser.roles.includes("Card creator") || this.existingUser.roles.includes("Lector") || this.existingUser.roles.includes("Graphic") || this.existingUser.roles.includes("Professional reviewer") || this.existingUser.roles.includes("Coordinator") 
      || this.existingUser.roles.includes("Main Lector") || this.existingUser.roles.includes("Main Graphic") || this.existingUser.roles.includes("Main Professional reviewer"));
    }

    get isUserWithTwoRole() {
      return this.existingUser && this.existingUser.roles.length > 1;
    }

    get isEditRoleTitle() {
      return this.actionType == "Szerkesztendő" && this.existingUser;
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

    roleToString2(role) {      
        for(let item of this.roles) {
          if(item.key == role) {
            return item.value;
          }
        }  
    }

    save() {
      this.submitted = true;
        if (this.form.invalid) {
          return;
        }
        // New
        if (this.actionType === 'Új') {
            let rolesList = new Array<string>();
            rolesList.push(this.form.get(this.formRole).value);
            let user: User = {
                id: "",
                name: this.form.get(this.formName).value,
                phone: this.form.get(this.formPhone).value,
                email: this.form.get(this.formEmail).value,
                password: this.form.get(this.formPassword).value,    
                password_again: this.form.get(this.formPassword).value,    
                workplace: this.form.get(this.formWorkplace).value,
                create_module: Number(this.form.get(this.formCreate_module).value),
                will_create_module: Number(this.form.get(this.formWill_create_module).value),
                roles: rolesList,
                activated: true
            };

            this.loading = true;
            this.userService.addUser(user)
                .subscribe((data) => {
                  this.router.navigate(['/user-list']);
                },
                error => {
                  this.error = "Ezzel az emailcímmel felhasználó már létezik!";
                  this.loading = false;
              });
        }
        // Edit
        if (this.actionType === 'Szerkesztendő') {
            let user: User = {
              id: this.existingUser.id,
              name: this.form.get(this.formName).value, 
              phone: this.form.get(this.formPhone).value, 
              email: this.form.get(this.formEmail).value, 
              password: this.form.get(this.formPassword).value,  
              password_again: this.form.get(this.formPassword).value,            
              workplace: this.form.get(this.formWorkplace).value,
              create_module: Number(this.form.get(this.formCreate_module).value),
              will_create_module: Number(this.form.get(this.formWill_create_module).value),
              roles: this.existingUser.roles,
              activated: true
            };

            this.userService.updateUser(user.id, user)
              .subscribe((data) => {
                this._location.back();
              });
        }
    }

    addPlusRole() {
      let rolesList = new Array<string>();
      rolesList.push(this.existingUser.roles[0]);
      rolesList.push(this.plusForm.get(this.formPlusRole).value);
      let user: User = {
        id: this.existingUser.id,
        name: this.form.get(this.formName).value, 
        phone: this.form.get(this.formPhone).value, 
        email: this.form.get(this.formEmail).value, 
        password: this.form.get(this.formPassword).value,  
        password_again: this.form.get(this.formPassword).value,            
        workplace: this.form.get(this.formWorkplace).value,
        create_module: Number(this.form.get(this.formCreate_module).value),
        will_create_module: Number(this.form.get(this.formWill_create_module).value),
        roles: rolesList,
        activated: true
      };

      this.userService.addUserRole(user)
          .subscribe((data) => {
            this._location.back();
          });
    }

    delete(role) {
      const ans = confirm('Biztosan törölni akarod ezt a szerepkört?');
      if (ans) {        
        this.userService.deleteUserRole(this.existingUser.id, role).subscribe((data) => {
          this.loadUser();
        });
      }
    }
    
      get name() { return this.form.get(this.formName); }
      get phone() { return this.form.get(this.formPhone); }
      get email() { return this.form.get(this.formEmail); }
      get password() { return this.form.get(this.formPassword); }
      get password_again() { return this.form.get(this.formPassword); }
      get workplace() { return this.form.get(this.formWorkplace); }
      get create_module() { return this.form.get(this.formCreate_module); }
      get will_create_module() { return this.form.get(this.formWill_create_module); }
      get role() { return this.form.get(this.formRole); }
      get plus_role() { return this.plusForm.get(this.formPlusRole); }
    }
