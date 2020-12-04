import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Card } from '../models/card';

/**
 * CardService
 */
@Injectable({
    providedIn: 'root'
})
export class CardService {

    myAppUrl: string;
    myApiUrl: string;
    httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json; charset=utf-8'
        })
    };
    constructor(private http: HttpClient) {
        this.myAppUrl = `${environment.apiBaseUrl}/`;
        this.myApiUrl = 'api/Cards/';
    }

    getCards(): Observable<Card[]> {
        return this.http.get<Card[]>(this.myAppUrl + this.myApiUrl)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getCardsByDeck(id: number): Observable<Card[]> {
        return this.http.get<Card[]>(this.myAppUrl + this.myApiUrl + "CardsByDeck/" + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getCard(id: number): Observable<Card> {
        return this.http.get<Card>(this.myAppUrl + this.myApiUrl + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getCountCard(id: number): Observable<number> {
        return this.http.get<number>(this.myAppUrl + this.myApiUrl + "CountGetCard/" + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    addCard(card): Observable<Card> {
        return this.http.post<Card>(this.myAppUrl + this.myApiUrl, JSON.stringify(card), this.httpOptions)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    updateCard(id: number, card): Observable<Card> {
        return this.http.put<Card>(this.myAppUrl + this.myApiUrl + id, JSON.stringify(card), this.httpOptions)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    deleteCard(id: number): Observable<Card> {
        return this.http.delete<Card>(this.myAppUrl + this.myApiUrl + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    deleteCardPicture(id: number, type: string): Observable<Card> {
        return this.http.put<Card>(this.myAppUrl + this.myApiUrl + "DeleteCardPicture/" + id + "/" + type, this.httpOptions)
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
