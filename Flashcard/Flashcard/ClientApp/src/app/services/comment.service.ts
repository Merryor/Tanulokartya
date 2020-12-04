import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Comment } from '../models/comment';

/**
 * CommentService
 */
@Injectable({
    providedIn: 'root'
})
export class CommentService {

    myAppUrl: string;
    myApiUrl: string;
    httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json; charset=utf-8'
        })
    };
    constructor(private http: HttpClient) {
        this.myAppUrl = `${environment.apiBaseUrl}/`;
        this.myApiUrl = 'api/Comments/';
    }

    getComments(cardId: number): Observable<Comment[]> {
        return this.http.get<Comment[]>(this.myAppUrl + this.myApiUrl + "GetCommentsByCardId/" + cardId)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getComment(id: number): Observable<Comment> {
        return this.http.get<Comment>(this.myAppUrl + this.myApiUrl + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    getCountComment(id: number): Observable<number> {
        return this.http.get<number>(this.myAppUrl + this.myApiUrl + "CountGetComment/" + id)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    addComment(comment): Observable<Comment> {
        return this.http.post<Comment>(this.myAppUrl + this.myApiUrl, JSON.stringify(comment), this.httpOptions)
            .pipe(
                retry(1),
                catchError(this.errorHandler)
            );
    }

    deleteComment(id: string): Observable<Comment> {
        return this.http.delete<Comment>(this.myAppUrl + this.myApiUrl + id)
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
