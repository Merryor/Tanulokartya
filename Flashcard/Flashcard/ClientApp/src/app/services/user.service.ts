import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';

/**
 * UserService
 */
@Injectable({
    providedIn: 'root'
})
export class UserService {

    myAppUrl: string;
    myApiUrl: string;
    httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json; charset=utf-8'
        })
    };
    constructor(private http: HttpClient) {
        this.myAppUrl = `${environment.apiBaseUrl}/`;
        this.myApiUrl = 'api/ApplicationUsers/';
    }

    getUsers(): Observable<User[]> {
        return this.http.get<User[]>(this.myAppUrl + this.myApiUrl)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getUser(id: string): Observable<User> {
        return this.http.get<User>(this.myAppUrl + this.myApiUrl + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getUserRole(id: string): Observable<string> {
        return this.http.get<string>(this.myAppUrl + this.myApiUrl + "GetUserRole/" + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getUsersByRole(role: string): Observable<User[]> {
        return this.http.get<User[]>(this.myAppUrl + this.myApiUrl + "UsersByRole/" + role)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    addUser(user): Observable<User> {
        return this.http.post<User>(this.myAppUrl + this.myApiUrl, JSON.stringify(user), this.httpOptions)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    addUserRole(user): Observable<User> {
        return this.http.post<User>(this.myAppUrl + this.myApiUrl + "AddUserRole", JSON.stringify(user), this.httpOptions)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    updateUser(id: string, user): Observable<User> {
        return this.http.put<User>(this.myAppUrl + this.myApiUrl + id, JSON.stringify(user), this.httpOptions)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    deactivateUser(id: string): Observable<User> {
        return this.http.delete<User>(this.myAppUrl + this.myApiUrl + "Deactivate/" + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    deleteUserRole(id: string, role: string): Observable<User> {
        return this.http.delete<User>(this.myAppUrl + this.myApiUrl + "DeleteUserRole/" + id + "/" + role)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    errorHandler(error) {
        let errorMessage = '';
        if (error.error instanceof ErrorEvent) {
            // Get client-side error
            errorMessage = error.error.message;
        } else {
            // Get server-side error
            errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
        }
        console.log(errorMessage);
        return throwError(errorMessage);
    }
}
