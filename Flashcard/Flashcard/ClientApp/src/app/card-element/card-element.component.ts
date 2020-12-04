import { Component, OnInit } from '@angular/core';
import { CardService } from '../services/card.service';
import { CommentService } from '../services/comment.service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Card } from '../models/card';
import { User } from '../models/user';
import { Comment } from '../models/comment';
import { Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from '../services/authentication.service';
import { environment } from 'src/environments/environment';

/**
 * CardElement Component
 */
@Component({
  selector: 'app-card-element',
  templateUrl: './card-element.component.html',
  styleUrls: ['./card-element.component.css']
})
export class CardElementComponent implements OnInit {

  card$: Observable<Card>;
  comments$: Observable<Comment[]>;
  comments: Comment[];
  id: number;
  question_picture: string;
  answer_picture: string;
  currentUser: User;
  
  form: FormGroup;
  formComment: string;

  constructor(private cardService: CardService, private commentService: CommentService, private formBuilder: FormBuilder, private avRoute: ActivatedRoute, private _location: Location, private authenticationService: AuthenticationService) {
    const idParam = 'id';
    this.formComment = 'comment';
    if (this.avRoute.snapshot.params[idParam]) {
      this.id = this.avRoute.snapshot.params[idParam];
    }

    this.authenticationService.currentUser.subscribe(x => this.currentUser = x); 

    this.form = this.formBuilder.group(
      { comment: ['', Validators.maxLength(200)] })
  }

  ngOnInit() {
    this.loadCard();
    this.loadComments();
  }

  loadCard() {
    this.card$ = this.cardService.getCard(this.id);

    this.cardService.getCard(this.id).subscribe(res => {
      this.question_picture = res.question_picture;
      this.answer_picture = res.answer_picture;
    });
  }

  loadComments() {
    this.comments$ = this.commentService.getComments(this.id);

    this.commentService.getComments(this.id).subscribe(res => {
      this.comments = res;
    });
  }

  isMyComment(id:number) {
    return this.comments.find(c => c.id==id && c.user.id == this.currentUser.id);
  }

  get isQuestionPicture() {
    return this.question_picture != "";
  }
  get isAnswerPicture() {
    return this.answer_picture != "";
  }

  public createImgPath = (serverPath: string) => {
      return `${environment.apiBaseUrl}/${serverPath}`;
  }

  backClicked() {
    this._location.back();
  }

  addComment() {
    let comment: Comment = {
      id: 1,
      cardId: Number(this.id),
      userId: this.currentUser.id,
      user: this.currentUser,
      comment_text: this.form.get(this.formComment).value,
      comment_time: new Date()
    };

    this.commentService.addComment(comment)
            .subscribe((data) => { 
              window.location.reload();
            });
  }

  delete(id) {
    const ans = confirm('Biztosan törölni akarod ezt a hozzászólást? id: ' + id);
    if (ans) {
      this.commentService.deleteComment(id).subscribe((data) => {
        this.loadComments();
      });
    }
  }
}
