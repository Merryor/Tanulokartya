import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { HttpEventType, HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
 
/**
 * UploadComponent
 */
@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {
  public progress: number;
  public message: string;
  public message_error: string;
  errorflag: boolean;
  @Output() public onUploadFinished = new EventEmitter();
 
  constructor(private http: HttpClient) { }
 
  ngOnInit() {
    this.errorflag = true;
  }
 
  /**
  * This function implements image upload with validation
  */
  public uploadFile = (files) => {
    if (files.length === 0) {
      return;
    }
    let reader = new FileReader();
    if (files && files.length > 0) {
      let file = files[0];

      // File extension validation
      this.message_error = 'A kép csak jpg, jpeg, vagy png kiterjesztésű lehet!';
      var _validFileExtensions = ["jpg", "jpeg", "png"];
      for (var j = 0; j < _validFileExtensions.length; j++) {
        var sCurExtension = _validFileExtensions[j];
        if (file.type.substr(file.type.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
          this.errorflag = false;
          this.message_error = '';
        }
      }
      if(this.errorflag == false) {
        let img = new Image();
        img.src = window.URL.createObjectURL( file );
        reader.readAsDataURL(file);
        reader.onload = () => {
          setTimeout(() => {
            const width = img.naturalWidth;
            const height = img.naturalHeight;
            
            // Image size validation
            window.URL.revokeObjectURL(img.src);
            if ( width > 600 || height > 600 ) {
              this.message = '';
              this.message_error = 'A kép maximális mérete 600x600 lehet!';
              this.errorflag = true; 
              return;
            } else {
              this.message_error = "";
              let fileToUpload = <File>files[0];
              const formData = new FormData();
              let date = new Date();
              let currentDate = date.getFullYear() + "" + (date.getMonth()+1) + "" + date.getDate() + "_" + date.getHours();
              let fileToUploadName = currentDate + "_" + fileToUpload.name;
              formData.append('file', fileToUpload, fileToUploadName);
          
              this.http.post(`${environment.apiBaseUrl}/api/Upload`, formData, {reportProgress: true, observe: 'events'})
              .subscribe(event => {
                if (event.type === HttpEventType.UploadProgress)
                  this.progress = Math.round(100 * event.loaded / event.total);
                else if (event.type === HttpEventType.Response) {
                  this.message_error = '';
                  this.message = 'Sikeres képfeltöltés!';
                  this.onUploadFinished.emit(event.body);
                  this.errorflag = true;
                }
              });
            }
          }, 2000);
        };
      }      
    }
  }
}
