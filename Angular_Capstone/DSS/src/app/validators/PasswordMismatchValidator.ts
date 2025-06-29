import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function passwordMissmatchValidator():ValidatorFn
{
    return(formGroup:AbstractControl):ValidationErrors|null=>{
        const password = formGroup.get('password')?.value;
        const Confirmpassword = formGroup.get('confirmPassword')?.value;
        if (password !== Confirmpassword) {
          return { passwordsMismatch: true };
        }
        return null;

    }
}
