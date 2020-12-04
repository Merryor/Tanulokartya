import { FormGroup } from '@angular/forms';

/**
 * This function is a custom validator to check character limit.
 */
export function maxLength(controlName: string, characterLimit: number) {
    return (formGroup: FormGroup) => {
       const control = formGroup.controls[controlName];
       // Define local variables
      var tinymax, tinylen, htmlcount;

      // Manually setting our max character limit
      tinymax = characterLimit;

      // Grabbing the length of the curent editors content
      var text = control.value;
      var tinylenText = text.replace(/(<([^>]+)>)/ig,"").replace(/[ ()]/g, '')
      .replaceAll(/&aacute;/g,'a').replaceAll(/&eacute;/g,'e').replaceAll(/&iacute;/g,'i').replaceAll(/&oacute;/g,'o').replaceAll(/&ouml;/g,'o').replaceAll(/&#337;/g,'o').replaceAll(/&uacute;/g,'u').replaceAll(/&uuml;/g,'u').replaceAll(/&#369;/g,'u')
      .replaceAll(/&Aacute;/g,'A').replaceAll(/&Eacute;/g,'E').replaceAll(/&Iacute;/g,'I').replaceAll(/&Oacute;/g,'O').replaceAll(/&Ouml;/g,'O').replaceAll(/&#336;/g,'O').replaceAll(/&Uacute;/g,'U').replaceAll(/&Uuml;/g,'U').replaceAll(/&#368;/g,'U');

      tinylen = tinylenText.length;
      htmlcount = "Karakterszám: " + tinylen + "/" + tinymax;

      if (control.errors && !control.errors.maxLength) {
        // Return if another validator has already found an error on the matchingControl
        return;
      }

      // If the user has exceeded the max turn the path bar red
      if (tinylen > tinymax){
        control.setErrors({ maxLength: true });
    } else {
        control.setErrors(null);
    }
  }
}