import { AbstractControl, ValidatorFn, Validators } from '@angular/forms';

export function customEmailValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
        const emailError = Validators.email(control);
        return control.value && emailError ? {'email': {}} : null;
    };
}
