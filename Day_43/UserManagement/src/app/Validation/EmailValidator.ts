import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function emailValidator():ValidatorFn
{
    return(control:AbstractControl):ValidationErrors|null=>{
        const value = control.value;
        if(value && !value.endsWith('@gmail.com'))
            return {invalidDomian: true};
        return null;

    }
}
