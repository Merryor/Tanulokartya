import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Deck } from '../models/deck';

/**
 * DeckService
 */
@Injectable({
    providedIn: 'root'
})
export class DeckService {

    myAppUrl: string;
    myApiUrl: string;
    httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json; charset=utf-8'
        })
    };
    constructor(private http: HttpClient) {
        this.myAppUrl = `${environment.apiBaseUrl}/`;
        this.myApiUrl = 'api/Decks/';
    }

    getDecks(): Observable<Deck[]> {
        return this.http.get<Deck[]>(this.myAppUrl + this.myApiUrl)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getMyDecks(userId: string): Observable<Deck[]> {
        return this.http.get<Deck[]>(this.myAppUrl + this.myApiUrl + "GetMyDecks/" + userId )
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getAllDecks(): Observable<Deck[]> {
        return this.http.get<Deck[]>(this.myAppUrl + this.myApiUrl + "GetAllDecks")
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getDeck(id: number): Observable<Deck> {
        return this.http.get<Deck>(this.myAppUrl + this.myApiUrl + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getDecksByModule(module: number): Observable<Deck[]> {
        return this.http.get<Deck[]>(this.myAppUrl + this.myApiUrl + "GetDecksByModule/" + module)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getCountDeck(id: number): Observable<number> {
        return this.http.get<number>(this.myAppUrl + this.myApiUrl + "CountGetDeck/" + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    addDeck(deck): Observable<Deck> {
        return this.http.post<Deck>(this.myAppUrl + this.myApiUrl, JSON.stringify(deck), this.httpOptions)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    updateDeck(id: number, deck): Observable<Deck> {
        return this.http.put<Deck>(this.myAppUrl + this.myApiUrl + id, JSON.stringify(deck), this.httpOptions)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }
    

    updateActivateDeck(id: number): Observable<Deck> {
        return this.http.put<Deck>(this.myAppUrl + this.myApiUrl + "UpdateActivate/" + id, this.httpOptions)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    updateStateDeck(id: number, state: number): Observable<Deck> {
        return this.http.put<Deck>(this.myAppUrl + this.myApiUrl + "UpdateState/" + id + "/" + state, this.httpOptions)
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
