import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DeckAssignment } from '../models/deckAssignment';

/**
 * DeckAssignmentService
 */
@Injectable({
    providedIn: 'root'
})
export class DeckAssignmentService {

    myAppUrl: string;
    myApiUrl: string;
    httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json; charset=utf-8'
        })
    };
    constructor(private http: HttpClient) {
        this.myAppUrl = `${environment.apiBaseUrl}/`;
        this.myApiUrl = 'api/DeckAssignments/';
    }

    getDeckAssignments(): Observable<DeckAssignment[]> {
        return this.http.get<DeckAssignment[]>(this.myAppUrl + this.myApiUrl)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getDeckAssignmentsByDeck(deckId: number): Observable<DeckAssignment[]> {
        return this.http.get<DeckAssignment[]>(this.myAppUrl + this.myApiUrl + "ByDeck/" + deckId)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    GetDeckAssignmentsByUser(userId: string): Observable<DeckAssignment[]> {
        return this.http.get<DeckAssignment[]>(this.myAppUrl + this.myApiUrl + "ByUser/" + userId)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getDeckAssignment(userId: string): Observable<DeckAssignment> {
        return this.http.get<DeckAssignment>(this.myAppUrl + this.myApiUrl + userId)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getIsRoleOk(deckId: number, roleName: string): Observable<boolean> {
        return this.http.get<boolean>(this.myAppUrl + this.myApiUrl + "IsRoleOk/"+deckId+"/"+roleName)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    addDeckAssignment(deckAssignment): Observable<DeckAssignment> {
        return this.http.post<DeckAssignment>(this.myAppUrl + this.myApiUrl, JSON.stringify(deckAssignment), this.httpOptions)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    deleteDeckAssignment(deckId: number, userId: string): Observable<DeckAssignment> {
        return this.http.delete<DeckAssignment>(this.myAppUrl + this.myApiUrl + deckId + "/" + userId)
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
