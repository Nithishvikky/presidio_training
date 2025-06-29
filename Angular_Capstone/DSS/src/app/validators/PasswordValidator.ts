import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function passwordValidator():ValidatorFn
{
    return(control:AbstractControl):ValidationErrors|null=>{
        const value = control.value;


        const hasNumber = /\d/.test(value);
        const hasSymbol = /[!@#$%^&*(),.?":{}|<>]/.test(value);

        const valid = value && value.length >= 8 && hasNumber && hasSymbol;

        return valid ? null : { passwordStrength: true };
    }
}
